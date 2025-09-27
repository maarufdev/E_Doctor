using E_Doctor.Application.DTOs.Common;
using E_Doctor.Application.DTOs.Common.CustomResultDTOs;
using E_Doctor.Application.DTOs.Common.ExportIllnessDTOs;
using E_Doctor.Application.DTOs.Diagnosis;
using E_Doctor.Application.DTOs.Settings.Symptoms;
using E_Doctor.Application.Interfaces.Features.Common;
using E_Doctor.Application.Interfaces.Features.Patient.Diagnosis;
using E_Doctor.Core.Domain.Entities.Patient;
using E_Doctor.Infrastructure.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace E_Doctor.Application.Services.Patient.Diagnosis
{
    internal class PatientService : IPatientService
    {
        private readonly PatientAppDbContext _appDbContext;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IUserManagerService _userServices;

        public PatientService(
            PatientAppDbContext context,
            IUserManagerService userServices)
        {
            _appDbContext = context;
            _userServices = userServices;
        }

        public async Task<List<DiagnosisListDTO>> GetDiagnosis()
        {
            var userId = await _userServices.GetUserId();
            var diagnosis = await _appDbContext.PatientDiagnosis
                .AsNoTracking()
                .Where(d => d.IsActive && d.UserId == userId)
                .OrderByDescending(d => d.UpdatedOn ?? d.CreatedOn)
                .Select(d => new
                {
                    d.Id,
                    UpdatedOn = d.UpdatedOn ?? d.CreatedOn,
                    Symptoms = string.Join(", ", d.DiagnosSymptoms.Select(s => $"{s.SymptomName} {s.Days} Days").ToList()),
                    IllnessName = string.Join(", ", d.DiagnosIllnesses.Select(i => $"{i.Illness} {i.Score.ToString("F2")}%").ToList()),
                })
                .Select(d => new DiagnosisListDTO(
                    d.Id,
                    d.UpdatedOn,
                    d.Symptoms,
                    d.IllnessName,
                    string.Empty
                 ))
                .ToListAsync();

            return diagnosis;
        }

        public async Task<IEnumerable<GetSymptomDTO>> GetSymtoms()
        {
            return await _appDbContext.PatientSymptoms
                .Select(s => new GetSymptomDTO(s.SymptomId, s.SymptomName)).ToListAsync();
        }

        public async Task<bool> MigrateRules(string jsonData)
        {
            try
            {
                var config = JsonSerializer.Deserialize<ExportIllnessConfigDTO>(jsonData);

                if (config is null) return false;

                await _appDbContext.Database.ExecuteSqlRawAsync("DELETE FROM PatientSymptoms");
                await _appDbContext.Database.ExecuteSqlRawAsync("DELETE FROM PatientIllnesses");
                await _appDbContext.Database.ExecuteSqlRawAsync("DELETE FROM PatientIllnesRules");

                var newSymptoms = config.Symptoms
                    .Select(s => new PatientSymptomEntity
                    {
                        SymptomId = s.SymptomId,
                        SymptomName = s.SymptomName,
                        UpdatedOn = DateTime.UtcNow,
                    });

                await _appDbContext.PatientSymptoms.AddRangeAsync(newSymptoms);

                var newIllnesses = config.Illnesses
                    .Select(i => new PatientIllnessEntity
                    {
                        IllnessId = i.IllnessId,
                        IllnessName = i.IllnessName,
                        Description = i.Description,
                        Prescription = i.Prescription,
                        Notes = i.Notes,
                        UpdatedOn = DateTime.UtcNow,
                    });

                await _appDbContext.PatientIllnesses.AddRangeAsync(newIllnesses);

                var saveMasterTableStatus = await _appDbContext.SaveChangesAsync() > 0;

                if (!saveMasterTableStatus)
                {
                    throw new InvalidOperationException("Something went wrong");
                }

                var toExportIllnessRuleId = config.Rules.Select(r => r.IllnessId).Distinct().ToList();

                var toUpdateIllnesses = await _appDbContext.PatientIllnesses
                    .Where(i => toExportIllnessRuleId.Contains(i.IllnessId))
                    .ToListAsync();

                foreach(var illness in toUpdateIllnesses)
                {
                    illness.Rules = config.Rules
                        .Where(r => r.IllnessId == illness.IllnessId)
                        .Select(r => new PatientRulesEntity
                        {
                            SymptomId = r.SymptomId,
                            IllnessId = illness.IllnessId,
                            Condition = r.Condition,
                            Days = r.Days,
                            Weight = r.Weight,
                        })
                        .ToList();
                }

                var result = await _appDbContext.SaveChangesAsync() > 0;

                return result;
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
                
                await _appDbContext.Database.ExecuteSqlRawAsync("DELETE FROM PatientSymptoms");
                await _appDbContext.Database.ExecuteSqlRawAsync("DELETE FROM PatientIllnesses");
                await _appDbContext.Database.ExecuteSqlRawAsync("DELETE FROM PatientIllnesRules");

                throw;
            }
        }

        public async Task<DiagnosisDetailsDTO> GetDiagnosisById(int diagnosisId)
        {
            var result = new DiagnosisDetailsDTO();

            var illness = await _appDbContext.PatientDiagnosisIllnesses
                .AsNoTracking()
                .Where(i => i.IsActive && i.DiagnosisId == diagnosisId)
                .FirstOrDefaultAsync();

            if (illness == null) return result;

            return DiagnosisDetailsDTO.Create(
                illness.Illness ?? string.Empty,
                illness.Description ?? string.Empty,
                illness.Prescription ?? string.Empty,
                illness.Notes ?? string.Empty);
        }

        public async Task<Result<DiagnosisDetailsDTO>> RunDiagnosis(RunDiagnosisDTO diagnosisRequest)
        {
            try
            {
                var userId = await _userServices.GetUserId();
                var result = new List<DiagnosisResultDTO>();
                var diagnosisResult = new DiagnosisDetailsDTO();

                var illness = await _appDbContext.PatientIllnesses
                   .AsNoTracking()
                   .Include(r => r.Rules)
                   .ThenInclude(s => s.Symptom)
                   .Where(i => i.IllnessId == diagnosisRequest.IllnessId)
                   .Select(i => new
                   {
                       IllnessId = i.IllnessId,
                       i.IllnessName,
                       i.Prescription,
                       i.Description,
                       i.Notes,
                       Rules = i.Rules.ToList(),
                       RuleCount = i.Rules.Count(),
                       MatchedRules = i.Rules.Where(r => diagnosisRequest.SymptomIds.Contains(r.SymptomId)).ToList(),
                       MatchedRuleCount = i.Rules.Count(r => diagnosisRequest.SymptomIds.Contains(r.SymptomId)),
                   })
                   .FirstOrDefaultAsync();

                if (illness is null) return Result<DiagnosisDetailsDTO>.Failure("Illness cannot be found.");

                var score = illness.RuleCount / illness.MatchedRuleCount * 100;

                var toSaveDiagnosis = new PatientDiagnosisEntity
                {
                    CreatedOn = DateTime.UtcNow,
                    DiagnosIllnesses = new List<PatientDiagnosisIllnessEntity>(),
                    DiagnosSymptoms = new List<PatientDiagnosisSymptomEntity>(),
                    IsActive = true,
                    UserId = userId
                };

                var patientIllness = new PatientDiagnosisIllnessEntity
                {
                    DiagnosisId = toSaveDiagnosis.Id,
                    Illness = illness.IllnessName,
                    Description = illness.Description,
                    Prescription = illness.Prescription ?? string.Empty,
                    Notes = illness.Notes,
                    Score = score,
                    IsActive = true,
                    CreatedOn = DateTime.Now,
                };

                toSaveDiagnosis.DiagnosIllnesses.Add(patientIllness);

                foreach (var symptom in illness.MatchedRules) 
                {
                    toSaveDiagnosis.DiagnosSymptoms.Add(new PatientDiagnosisSymptomEntity
                    {
                        DiagnosisId = toSaveDiagnosis.Id,
                        SymptomName = symptom.Symptom?.SymptomName ?? string.Empty,
                        Days = 0,
                        IsActive = true,
                        CreatedOn = DateTime.UtcNow,
                    });
                }

                await _appDbContext.PatientDiagnosis.AddAsync(toSaveDiagnosis);
                await _appDbContext.SaveChangesAsync();

                diagnosisResult = DiagnosisDetailsDTO.Create(
                       patientIllness.Illness,
                       patientIllness.Description ?? string.Empty,
                       patientIllness.Prescription ?? string.Empty,
                       patientIllness.Notes ?? string.Empty
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
            return await _appDbContext.PatientIllnesses
                .AsNoTracking()
                .Select(i => new GetConsultationIllnessDTO(i.IllnessId, i.IllnessName))
                .ToListAsync();
        }

        public async Task<Result<List<GetConsultationSymptomByIllnessIdDTO>>> GetConsultationSymptomByIllnessId(int illnessId)
        {
            var illness = await _appDbContext.PatientIllnesses
                .AsNoTracking()
                .Include(r => r.Rules)
                .ThenInclude(s => s.Symptom)
                .Where(i => i.IllnessId == illnessId)
                .Select(i => new
                {
                    IllnessId = i.IllnessId,
                    Symptoms = i.Rules.Select(s => s.Symptom).ToList()
                })
                .FirstOrDefaultAsync();

            if (illness == null) return Result<List<GetConsultationSymptomByIllnessIdDTO>>.Failure("Illness Symptoms available.");

            var symptoms = illness.Symptoms
                .Select(s => new GetConsultationSymptomByIllnessIdDTO(s.SymptomId, s.SymptomName))
                .ToList();

            return Result<List<GetConsultationSymptomByIllnessIdDTO>>.Success(symptoms);
        }
    }
}
