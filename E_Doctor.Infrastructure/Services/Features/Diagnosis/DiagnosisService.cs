using E_Doctor.Application.DTOs.Common;
using E_Doctor.Application.DTOs.Common.CustomResultDTOs;
using E_Doctor.Application.DTOs.Diagnosis;
using E_Doctor.Application.Interfaces.Features.Common;
using E_Doctor.Application.Interfaces.Features.Diagnosis;
using E_Doctor.Application.Interfaces.Features.UserActivity;
using E_Doctor.Core.Constants.Enums;
using E_Doctor.Core.Domain.Entities.Admin;
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
                .Join(_appDbContext.Users, d => d.UserId, u => u.Id, (Diagnosis, User) => new { User, Diagnosis })
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
                    IllnessName = string.Join(", ", d.Diagnosis.DiagnosIllnesses.Select(i => $"{i.Illness} {i.Score.ToString("F2")}%").ToList()),
                    UserId = d.User.IsActive,
                    FullName = $"{d.User.FirstName} {d.User.LastName}"
                })
                .Select(d => new DiagnosisListResponse(
                    d.Id,
                    d.UpdatedOn,
                    d.FullName,
                    d.Symptoms,
                    d.IllnessName,
                    string.Empty
                 ))
                .ToListAsync();

            return new PagedResult<DiagnosisListResponse>
            {
                Items = diagnosisResult,
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
        var result = new DiagnosisDetailsDTO();

        var illness = await _appDbContext.DiagnosisIllnesses
            .AsNoTracking()
            .Where(i => i.IsActive && i.DiagnosisId == diagnosisId)
            .FirstOrDefaultAsync();

        if (illness == null) return result;

        return DiagnosisDetailsDTO.Create(
            $"{illness.Illness} {illness.Score.ToString("F2")}%" ?? string.Empty,
            illness.Description ?? string.Empty,
            illness.Prescription ?? string.Empty,
            illness.Notes ?? string.Empty);
    }
    public async Task<Result<DiagnosisDetailsDTO>> RunDiagnosis(RunDiagnosisDTO diagnosisRequest)
    {
        try
        {
            int? userId = await _userManager.GetUserId();
            var userName = await _userManager.GetUserNameById(userId.ToString());

            var result = new List<DiagnosisResultDTO>();
            var diagnosisResult = new DiagnosisDetailsDTO();

            var illness = await _appDbContext.Illnesses
               .AsNoTracking()
               .Include(r => r.Rules)
               .ThenInclude(s => s.Symptom)
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
               .OrderByDescending(o => o.MatchedRuleCount)
               .FirstOrDefaultAsync();

            if (illness is null) return Result<DiagnosisDetailsDTO>.Failure("Illness cannot be found.");

            var score = (double)illness.MatchedRuleCount / illness.RuleCount * 100;

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
                Illness = illness.IllnessName,
                Description = illness.Description,
                Prescription = illness.Prescription ?? string.Empty,
                Notes = illness.Notes,
                Score = (decimal)score,
                IsActive = true,
                CreatedOn = DateTime.Now,
            };

            toSaveDiagnosis.DiagnosIllnesses.Add(patientIllness);

            foreach (var symptom in illness.MatchedRules)
            {
                toSaveDiagnosis.DiagnosSymptoms.Add(new DiagnosisSymptomsEntity
                {
                    DiagnosisId = toSaveDiagnosis.Id,
                    SymptomName = symptom.Symptom?.Name ?? string.Empty,
                    IsActive = true,
                    CreatedOn = DateTime.UtcNow,
                });
            }

            await _appDbContext.Diagnosis.AddAsync(toSaveDiagnosis);
            await _appDbContext.SaveChangesAsync();

            diagnosisResult = DiagnosisDetailsDTO.Create(
                   $"{patientIllness.Illness} {score.ToString("F2")}%",
                   patientIllness.Description ?? string.Empty,
                   patientIllness.Prescription ?? string.Empty,
                   patientIllness.Notes ?? string.Empty
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
}