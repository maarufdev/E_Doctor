using E_Doctor.Application.DTOs.Common.ExportIllnessDTOs;
using E_Doctor.Application.DTOs.Diagnosis;
using E_Doctor.Application.DTOs.Settings.Symptoms;
using E_Doctor.Application.Interfaces.Features.Common;
using E_Doctor.Application.Interfaces.Features.Patient.Diagnosis;
using E_Doctor.Core.Constants.Enums;
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
                .Include(d => d.DiagnosSymptoms)
                .Include(d => d.DiagnosIllnesses)
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
                    d.IllnessName
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

        public async Task<List<DiagnosisResultDTO>> RunDiagnosis(List<RunDiagnosisDTO> diagnosisRequest)
        {
            try
            {
                var userId = await _userServices.GetUserId();

                var result = new List<DiagnosisResultDTO>();
                var patientSymptomsIds = diagnosisRequest.Select(s => s.SymptomId).ToList();

                var potentialIllnesses = await _appDbContext.PatientIllnesses
                    .AsNoTracking()
                    .Include(i => i.Rules)
                    .Where(i => i.Rules != null && i.Rules.Any(r => patientSymptomsIds.Contains(r.SymptomId)))
                    .Select(i => new
                    {
                        IllnessId = i.IllnessId,
                        i.IllnessName,
                        MatchingRules = i.Rules.Where(r => patientSymptomsIds.Contains(r.SymptomId)).ToList(),
                        MaxScore = i.Rules.Sum(r => (int)r.Weight)
                    })
                    .ToListAsync();

                var illnessScores = new Dictionary<int, decimal>();
                var patientSymptomLookup = diagnosisRequest.ToDictionary(d => d.SymptomId, d => d.Duration);

                foreach (var illness in potentialIllnesses)
                {
                    decimal currentScore = 0;

                    foreach (var rule in illness.MatchingRules)
                    {
                        if (patientSymptomLookup.TryGetValue(rule.SymptomId, out var patientDuration))
                        {
                            bool isRuleMet = false;

                            switch (rule.Condition)
                            {
                                case IllnessRuleConditionEnum.IsEqual:
                                    isRuleMet = patientDuration == rule.Days;
                                    break;

                                case IllnessRuleConditionEnum.IsLessThan:
                                    isRuleMet = patientDuration < rule.Days;
                                    break;

                                case IllnessRuleConditionEnum.IsLessThanOrEqual:
                                    isRuleMet = patientDuration <= rule.Days;
                                    break;

                                case IllnessRuleConditionEnum.IsMoreThan:
                                    isRuleMet = patientDuration > rule.Days;
                                    break;

                                case IllnessRuleConditionEnum.IsMoreThanOrEqual:
                                    isRuleMet = patientDuration >= rule.Days;
                                    break;
                            }

                            if (isRuleMet)
                            {
                                currentScore += (int)rule.Weight;
                            }
                        }
                    }

                    if (currentScore > 0)
                    {
                        illnessScores[illness.IllnessId] = currentScore / illness.MaxScore * 100;
                    }
                }

                var top5Illnesses = illnessScores
                    .OrderByDescending(kvp => kvp.Value)
                    .Take(5);

                foreach (var illnessScore in top5Illnesses)
                {
                    var illness = potentialIllnesses.FirstOrDefault(i => i.IllnessId == illnessScore.Key);

                    if (illness is null) continue;

                    if (illness.MaxScore > 0)
                    {
                        var percentageToDisplay = illnessScore.Value.ToString("F2");

                        result.Add(new DiagnosisResultDTO(illness.IllnessName, percentageToDisplay));
                    }
                }

                if (top5Illnesses.Any())
                {
                    var toSaveDiagnosis = new PatientDiagnosisEntity
                    {
                        CreatedOn = DateTime.UtcNow,
                        DiagnosIllnesses = new List<PatientDiagnosisIllnessEntity>(),
                        DiagnosSymptoms = new List<PatientDiagnosisSymptomEntity>(),
                        IsActive = true,
                        UserId = userId
                    };

                    foreach (var diagnosis in top5Illnesses)
                    {
                        var diagnosIllness = potentialIllnesses.FirstOrDefault(x => x.IllnessId == diagnosis.Key);
                        
                        if(diagnosIllness is null) continue;

                        toSaveDiagnosis.DiagnosIllnesses.Add(new PatientDiagnosisIllnessEntity
                        {
                            DiagnosisId = toSaveDiagnosis.Id,
                            Illness = diagnosIllness.IllnessName,
                            Score = diagnosis.Value,
                            IsActive = true,
                            CreatedOn = DateTime.Now,
                        });
                    }

                    var symptoms = await _appDbContext.PatientSymptoms
                            .AsNoTracking()
                            .Where(s => patientSymptomsIds.Contains(s.SymptomId))
                            .ToListAsync();

                    foreach (var symptom in symptoms) 
                    {
                        toSaveDiagnosis.DiagnosSymptoms.Add(new PatientDiagnosisSymptomEntity
                        {
                            DiagnosisId = toSaveDiagnosis.Id,
                            SymptomName = symptom.SymptomName,
                            Days = patientSymptomLookup[symptom.SymptomId],
                            IsActive = true,
                            CreatedOn = DateTime.UtcNow,
                        });
                    }

                    await _appDbContext.PatientDiagnosis.AddAsync(toSaveDiagnosis);
                    await _appDbContext.SaveChangesAsync();
                }

                return result;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                throw;
            }
        }
    }
}
