using E_Doctor.Application.DTOs.Common;
using E_Doctor.Application.DTOs.Common.CustomResultDTOs;
using E_Doctor.Application.DTOs.Diagnosis;
using E_Doctor.Application.DTOs.Diagnosis.PhysicalExams;
using E_Doctor.Application.Interfaces.Features.Common;
using E_Doctor.Application.Interfaces.Features.Diagnosis;
using E_Doctor.Core.Constants.Enums;
using E_Doctor.Core.Domain.Entities.Admin;
using E_Doctor.Core.Domain.Entities.Patient.PhysicalExam;
using E_Doctor.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace E_Doctor.Infrastructure.Services.Features.Diagnosis;

internal class DiagnosisService : IDiagnosisService
{
    private readonly AppDbContext _appDbContext;
    private readonly IUserManagerService _userManager;
    private readonly IActivityLoggerService _activityLoggerService;
    public DiagnosisService(
        AppDbContext appDbContext,
        IUserManagerService userManager,
        IActivityLoggerService activityLoggerService)
    {
        _appDbContext = appDbContext;
        _userManager = userManager;
        _activityLoggerService = activityLoggerService;
    }

    public async Task<PagedResult<DiagnosisListResponse>> GetDiagnosis(GetDiagnosisParamsDTO requestParams)
    {
        try
        {
            var query = _appDbContext.Diagnosis
                .AsNoTracking()
                .Include(d => d.DiagnosIllnesses)
                .Include(d => d.DiagnosSymptoms)
                .Join(
                    _appDbContext.Users, 
                    d => d.UserId, 
                    u => u.Id, 
                    (Diagnosis, User) => new { User, Diagnosis })
                .GroupJoin(
                    _appDbContext.PatientInformations,
                    du => du.User.Id,
                    p => p.UserId,
                    (du, p) => new { du, PatientInformation = p })
                .SelectMany(
                    x => x.PatientInformation.DefaultIfEmpty(),
                    (x, p) => new
                    {
                        x.du.User,
                        x.du.Diagnosis,
                        PatientInformation = p
                    })
                .Where(x => x.User.IsActive && x.Diagnosis.IsActive);

            var isAdmin = await _userManager.IsUserAdmin();

            if (!isAdmin)
            {
                var currentUserId = await _userManager.GetUserId();
                if(currentUserId != null)
                {
                    query = query.Where(u => u.User.Id == currentUserId);
                }
            }
            else
            {
                if (!string.IsNullOrEmpty(requestParams.SearchText))
                {
                    var searchText = requestParams.SearchText.ToLower();

                    query = query.Where(u =>
                        u.User.UserName.ToLower().Contains(searchText) ||
                        u.User.FirstName.ToLower().Contains(searchText) ||
                        u.User.LastName.ToLower().Contains(searchText) ||
                        u.User.MiddleName.ToLower().Contains(searchText)
                    );
                }
            }

                query = query.OrderByDescending(d => d.Diagnosis.UpdatedOn ?? d.Diagnosis.CreatedOn);
            var totalCount = await query.CountAsync();
            var response = new List<DiagnosisListResponse>();

            var diagnosisResult = await query
                .Skip((requestParams.PageNumber - 1) * requestParams.PageSize)
                .Take(requestParams.PageSize)
                .Select(d => new
                {
                    d.Diagnosis.Id,
                    UpdatedOn = d.Diagnosis.UpdatedOn ?? d.Diagnosis.CreatedOn,
                    Symptoms = string.Join(", ", d.Diagnosis.DiagnosSymptoms.Select(s => $"{s.SymptomName}").ToList()),
                    IllnessName = string.Join(", ", d.Diagnosis.DiagnosIllnesses.Select(i => $"{i.Illness}").ToList()),
                    //IllnessName = string.Join(", ", d.Diagnosis.DiagnosIllnesses.Select(i => $"{i.Illness} {i.Score.ToString("F2")}%").ToList()),
                    UserId = d.User.IsActive,
                    FullName = $"{d.User.FirstName} {d.User.LastName}",
                    PatientInfoId = d.PatientInformation != null ? d.PatientInformation.Id : 0

                })
                .ToListAsync();

            var diagnosisIds = diagnosisResult.Select(x => x.Id).ToList();
            
            List<DiagnosisListResponse> diagnosisListResponse = new();

            foreach(var item in diagnosisResult)
            {
                var examId = await _appDbContext.PhysicalExams
                    .AsNoTracking()
                    .Where(p => p.DiagnosisId == item.Id && p.IsActive)
                    .Select(p => p.Id)
                    .FirstOrDefaultAsync();

                diagnosisListResponse.Add(new DiagnosisListResponse(
                    item.Id,
                    item.UpdatedOn,
                    item.FullName,
                    item.Symptoms,
                    item.IllnessName,
                    string.Empty,
                    examId,
                    item.PatientInfoId
                    ));
            }


            return new PagedResult<DiagnosisListResponse>
            {
                Items = diagnosisListResponse,
                TotalCount = totalCount,
                PageSize = requestParams.PageSize,
                PageNumber = requestParams.PageNumber,
            };
        }
        catch(Exception ex)
        {
            Console.WriteLine(ex.ToString());
            throw;
        }
    }


    public async Task<DiagnosisDetailsDTO> GetDiagnosisById(int diagnosisId)
    {

        var illness = await _appDbContext.DiagnosisIllnesses
            .AsNoTracking()
            .Where(i => i.IsActive && i.DiagnosisId == diagnosisId)
            .FirstOrDefaultAsync();

        var selectedSymptoms = await _appDbContext.DiagnosisSymptoms
            .AsNoTracking()
            .Where(x => x.DiagnosisId == diagnosisId)
            .Select(x => x.SymptomName)
            .ToArrayAsync();

        selectedSymptoms = selectedSymptoms ?? Array.Empty<string>();

        if (illness is null) return DiagnosisDetailsDTO.Create();

        return DiagnosisDetailsDTO.Create(
            $"{illness?.Illness}" ?? string.Empty,
            illness?.Description ?? string.Empty,
            illness?.Prescription ?? string.Empty,
            illness?.Notes ?? string.Empty,
            selectedSymptoms
            );
    }
    public async Task<Result<DiagnosisDetailsDTO>> RunDiagnosis(RunDiagnosisDTO diagnosisRequest)
    {
        try
        {
            int? userId = await _userManager.GetUserId();
            var userName = await _userManager.GetUserNameById(userId.ToString());

            var result = new List<DiagnosisResultDTO>();
            var diagnosisResult = DiagnosisDetailsDTO.Create();

            var potentialIllnessResult = await _appDbContext.Illnesses
               .AsNoTracking()
               .Include(r => r.Rules)
               .ThenInclude(s => s.Symptom)
               .Where(i => i.IsActive == true)
               .Select(i => new
               {
                   IllnessId = i.Id,
                   i.IllnessName,
                   i.Prescription,
                   i.Description,
                   i.Notes,
                   Rules = i.Rules.ToList(),
                   RuleCount = i.Rules.Count(),
                   MatchedRules = i.Rules.Where(r => diagnosisRequest.SymptomIds.Contains(r.SymptomId)).ToList(),
                   MatchedRuleCount = i.Rules.Count(r => diagnosisRequest.SymptomIds.Contains(r.SymptomId)),
               })
               .Where(i => i.MatchedRuleCount > 0)
               .OrderByDescending(o => o.MatchedRuleCount)
               .ToListAsync();

            if (!potentialIllnessResult.Any()) return Result<DiagnosisDetailsDTO>.Failure("Illness cannot be found. It’s advised to consult a doctor for a proper diagnosis.");

            var potentialResultWithScore = potentialIllnessResult
                .Select(illness =>
                {
                    var score = illness.RuleCount > 0
                        ? (double)illness.MatchedRuleCount / illness.RuleCount * 100
                        : 0;

                    return new
                    {
                        illness.IllnessId,
                        illness.IllnessName,
                        illness.Prescription,
                        illness.Description,
                        illness.Notes,
                        illness.MatchedRules,
                        Score = score
                    };
                })
                .OrderByDescending(i => i.Score)
                .ToList();

            var maxScore = potentialResultWithScore.Max(r => r.Score);
            var topResults = potentialResultWithScore.Where(r => Math.Abs(r.Score - maxScore) < 0.0001).ToList();
            var hasTie = (topResults.Count > 1);
            if (hasTie)
            {
                var tiedIllnesses = string.Join(", ", topResults.Select(r => r.IllnessName));
                diagnosisResult = DiagnosisDetailsDTO.Create(
                   "Multiple illnesses have similar match scores.",
                   $"Possible illnesses: {tiedIllnesses}.",
                   string.Empty,
                   "It’s advised to consult a doctor for a proper diagnosis.",
                   false
               );

                await _activityLoggerService.LogAsync(
                    userId,
                    UserActivityTypes.PatientDiagnosis,
                    diagnosisResult.Result ?? "Tie diagnosis - advised doctor consultation."
                );

                return Result<DiagnosisDetailsDTO>.Success(diagnosisResult);
            }

            var firstResult = topResults.First();
            var score = firstResult.Score;

            var toSaveDiagnosis = new DiagnosisEntity
            {
                CreatedOn = DateTime.UtcNow,
                DiagnosIllnesses = new List<DiagnosisIllnessesEntity>(),
                DiagnosSymptoms = new List<DiagnosisSymptomsEntity>(),
                IsActive = true,
                UserId = userId ?? 0
            };

            var patientIllness = new DiagnosisIllnessesEntity
            {
                DiagnosisId = toSaveDiagnosis.Id,
                Illness = firstResult.IllnessName,
                Description = firstResult.Description,
                Prescription = firstResult.Prescription ?? string.Empty,
                Notes = firstResult.Notes,
                Score = (decimal)score,
                IsActive = true,
                CreatedOn = DateTime.Now,
            };

            toSaveDiagnosis.DiagnosIllnesses.Add(patientIllness);

            var patientSymptoms = await _appDbContext.Symptoms
                .AsNoTracking()
                .Where(x => diagnosisRequest.SymptomIds.Contains(x.Id))
                .ToListAsync();

            //foreach (var symptom in firstResult.MatchedRules)
            //{
            //    toSaveDiagnosis.DiagnosSymptoms.Add(new DiagnosisSymptomsEntity
            //    {
            //        DiagnosisId = toSaveDiagnosis.Id,
            //        SymptomName = symptom.Symptom?.Name ?? string.Empty,
            //        IsActive = true,
            //        CreatedOn = DateTime.UtcNow,
            //    });
            //}

            foreach (var symptom in patientSymptoms)
            {
                toSaveDiagnosis.DiagnosSymptoms.Add(new DiagnosisSymptomsEntity
                {
                    DiagnosisId = toSaveDiagnosis.Id,
                    SymptomName = symptom.Name ?? string.Empty,
                    IsActive = true,
                    CreatedOn = DateTime.UtcNow,
                });
            }

            await _appDbContext.Diagnosis.AddAsync(toSaveDiagnosis);
            await _appDbContext.SaveChangesAsync();

            diagnosisResult = DiagnosisDetailsDTO.Create(
                   //$"{patientIllness.Illness} {score.ToString("F2")}%",
                   $"{patientIllness.Illness}",
                   patientIllness.Description ?? string.Empty,
                   patientIllness.Prescription ?? string.Empty,
                   patientIllness.Notes ?? string.Empty,
                   toSaveDiagnosis.DiagnosSymptoms?.Select(item => item.SymptomName).ToArray() ?? Array.Empty<string>()
                   );

            await _activityLoggerService
                .LogAsync(
                    userId,
                    UserActivityTypes.PatientDiagnosis,
                    diagnosisResult.Result ?? "Unknown"
                );
            return Result<DiagnosisDetailsDTO>.Success(diagnosisResult);
        }
        catch (Exception ex) 
        { 
            Console.WriteLine(ex.ToString());
            throw;
        }
    }

    public async Task<List<GetConsultationIllnessDTO>> GetConsultationIllnessList()
    {
        return await _appDbContext.Illnesses
            .AsNoTracking()
            .Where(i => i.IsActive)
            .Select(i => new GetConsultationIllnessDTO(i.Id, i.IllnessName))
            .ToListAsync();
    }

    public async Task<Result<List<GetConsultationSymptomByIllnessIdDTO>>> GetConsultationSymptomByIllnessId(int illnessId)
    {
        return Result<List<GetConsultationSymptomByIllnessIdDTO>>.Success(await _appDbContext.Symptoms
            .AsNoTracking()
            .Where(s => s.IsActive)
            .Select(s => new GetConsultationSymptomByIllnessIdDTO(s.Id, s.Name, s.QuestionText))
            .ToListAsync()
            );
    }

    public async Task<PagedResult<DiagnosisListByUserIdResponse>> GetDiagnosisByUserId(GetDiagnosisParamsDTO requestParams)
    {
        var userId = await _userManager.GetUserId();

        var query = _appDbContext.Diagnosis
            .AsNoTracking()
            .Where(d => d.IsActive && d.UserId == userId)
            .OrderByDescending(d => d.UpdatedOn ?? d.CreatedOn);

        var totalCount = await query.CountAsync();

        var diagnosis = await query
            .Skip((requestParams.PageNumber - 1) * requestParams.PageSize)
            .Take(requestParams.PageSize)
            .Select(d => new
            {
                d.Id,
                UpdatedOn = d.UpdatedOn ?? d.CreatedOn,
                Symptoms = string.Join(", ", d.DiagnosSymptoms.Select(s => $"{s.SymptomName}").ToList()),
                IllnessName = string.Join(", ", d.DiagnosIllnesses.Select(i => $"{i.Illness} {i.Score.ToString("F2")}%").ToList()),
            })
            .Select(d => new DiagnosisListByUserIdResponse(
                d.Id,
                d.UpdatedOn,
                d.Symptoms,
                d.IllnessName,
                string.Empty
             ))
            .ToListAsync();

        return new PagedResult<DiagnosisListByUserIdResponse>
        {
            Items = diagnosis,
            TotalCount = totalCount,
            PageSize = requestParams.PageSize,
            PageNumber = requestParams.PageNumber,
        };
    }

    public async Task<Result> DeleteDiagnosisById(int diagnosisId)
    {
        var diagnosis = await _appDbContext.Diagnosis.FirstOrDefaultAsync(x => x.Id == diagnosisId);
        
        if(diagnosis is null)
        {
            return Result.Success();
        }

        diagnosis.IsActive = false;
        diagnosis.UpdatedOn = DateTime.UtcNow;

        await _appDbContext.SaveChangesAsync();

        var userId = await _userManager.GetUserId();
        var userName = await _userManager.GetUserNameById(userId.ToString());

        var isAdmin = await _userManager.IsUserAdmin();
        var patientEmail = await _userManager.GetUserNameById(diagnosis.UserId.ToString());

        if (isAdmin)
        {
            await _activityLoggerService.LogAsync(
                userId,
                UserActivityTypes.AdminDeleteDiagnosis,
                userName
                );

            await _activityLoggerService.LogAsync(
                    diagnosis.UserId,
                    UserActivityTypes.AdminDeleteDiagnosis,
                    patientEmail
                    );
        } else
        {
            await _activityLoggerService.LogAsync(
                    diagnosis.UserId,
                    UserActivityTypes.PatientDeleteDiagnosis,
                    patientEmail
                    );
        }

         return Result.Success();
    }

    public async Task<Result<IEnumerable<PhysicalExamItemDTO>>> GetPhysicalItems()
    {
        try
        {
            var result = await _appDbContext.PhysicalExamItems
                .AsNoTracking()
                .Where(p => p.IsActive)
                .OrderBy(p => p.SortOrder)
                .Select(p => new PhysicalExamItemDTO(p.Id, p.Label))
                .ToListAsync();

            return Result<IEnumerable<PhysicalExamItemDTO>>.Success(result);
        }
        catch (Exception)
        {
            return Result<IEnumerable<PhysicalExamItemDTO>>.Failure("Something went wrong.");
        }
    }

    public async Task<Result<PhysicalExamDTO>> GetPhysicalExamById(int physicalExamId)
    {
        try
        {
            var result = await _appDbContext.PhysicalExams
            .Include(p => p.PhysicalExamFindings)
            .ThenInclude(p => p.PhysicalExamItem)
            .Where(p => p.Id == physicalExamId && p.IsActive)
            .FirstOrDefaultAsync();

            if (result is null) return Result<PhysicalExamDTO>.Failure("Physical exam report not found");

            var physicalExamDTO = new PhysicalExamDTO
            {
                ExamId = physicalExamId,
                BP = result.BP ?? string.Empty,
                HR = result.HR ?? string.Empty,
                RR = result.RR ?? string.Empty,
                Temp = result.Temp ?? string.Empty,
                Weight = result.Weight ?? string.Empty,
                O2Sat = result.O2Sat ?? string.Empty,
                PhysicalExamFindings = result.PhysicalExamFindings
                    .Select(p => new PhysicalExamFindingDTO
                    {
                        PhysicalExamId = p.PhysicalExamId,
                        PhysicalItemId = p.PhysicalItemId,
                        IsNormal = p.IsNormal,
                        NormalDescription = p.NormalDescription ?? string.Empty,
                        AbnormalFindings = p.AbnormalFindings ?? string.Empty
                    }).ToList()
            };

            return Result<PhysicalExamDTO>.Success(physicalExamDTO);
        }
        catch (Exception)
        {
            return Result<PhysicalExamDTO>.Failure("Something went wrong during physical exam report request.");
        }
    }

    public async Task<Result> SavePhysicalExamReport(SavePhysicalExamRequest request)
    {
        try
        {
            ArgumentNullException.ThrowIfNull(nameof(request));

            var diagnosisId = request.DiagnosisId;
            var physicalExamId = request.PhysicalExamId;

            if(physicalExamId == 0)
            {
                var physcalExamToCreate = new PhysicalExamEntity
                {
                    DiagnosisId = diagnosisId,
                    BP = request.BP,
                    HR = request.HR,
                    RR = request.RR,
                    Temp = request.Temp,
                    Weight = request.Weight,
                    O2Sat = request.O2Sat,
                    PhysicalExamFindings = new List<PhysicalExamFindingsEntity>(),
                    IsActive = true,
                    CreatedOn = DateTime.UtcNow
                };

                var physicalExamFindings = new List<PhysicalExamFindingsEntity>();

                foreach(var item in request.PhysicalExamFindings)
                {
                    var physicalExamFindingsToCreate = new PhysicalExamFindingsEntity
                    {
                        PhysicalItemId = item.PhysicalItemId,
                        IsNormal = item.IsNormal,
                        NormalDescription = item.NormalDescriptions,
                        AbnormalFindings = item.AbnormalFindings,
                        IsActive = true,
                        CreatedOn = DateTime.UtcNow
                    };

                    physicalExamFindings.Add(physicalExamFindingsToCreate);
                }

                physcalExamToCreate.PhysicalExamFindings = physicalExamFindings;

                await _appDbContext.PhysicalExams.AddAsync(physcalExamToCreate);
            } 
            else
            {
                var toUpdatePhysicalExam = await _appDbContext.PhysicalExams
                    .Include(p => p.PhysicalExamFindings)
                    .Where(p => p.Id == physicalExamId)
                    .OrderByDescending(p => p.CreatedOn ?? p.UpdatedOn)
                    .FirstOrDefaultAsync();

                if (toUpdatePhysicalExam is null) return Result.Failure("Unable to update a physical examination that don't exists.");

                toUpdatePhysicalExam.BP = request.BP;
                toUpdatePhysicalExam.HR = request.HR;
                toUpdatePhysicalExam.RR = request.HR;
                toUpdatePhysicalExam.Temp = request.HR;
                toUpdatePhysicalExam.Weight = request.HR;
                toUpdatePhysicalExam.O2Sat = request.HR;
                toUpdatePhysicalExam.UpdatedOn = DateTime.UtcNow;

                if(toUpdatePhysicalExam.PhysicalExamFindings is not null)
                {
                    var newFindings = request.PhysicalExamFindings;

                    foreach(var finding in toUpdatePhysicalExam.PhysicalExamFindings)
                    {
                        var newFindingUpdate = newFindings
                            .Where(x => x.PhysicalItemId == finding.PhysicalItemId)
                            .FirstOrDefault();

                        if (newFindingUpdate is null) continue;

                        finding.IsNormal = newFindingUpdate.IsNormal;
                        finding.NormalDescription = newFindingUpdate.NormalDescriptions;
                        finding.AbnormalFindings = newFindingUpdate.AbnormalFindings;
                        finding.UpdatedOn = DateTime.UtcNow;
                    }
                }

                _appDbContext.PhysicalExams.Update(toUpdatePhysicalExam);
            }

            var isSuccess = await _appDbContext.SaveChangesAsync() > 0;

            if (!isSuccess)
            {
                var operation = physicalExamId == 0 ? "creation" : "updating";

                return Result.Failure($"Something went wrong during {operation} of physical examination report" );
            }

            return Result.Success();
            
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.ToString());
            return Result.Failure("Something Went Wrong");
        }
    }
}