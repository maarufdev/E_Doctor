using E_Doctor.Application.DTOs.Diagnosis;
using E_Doctor.Application.Interfaces.Features.Admin.Diagnosis;
using E_Doctor.Application.Interfaces.Features.Common;
using E_Doctor.Core.Constants.Enums;
using E_Doctor.Core.Domain.Entities.Admin;
using E_Doctor.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace E_Doctor.Application.Services.Admin.Diagnosis;

internal class DiagnosisService : IDiagnosisService
{
    private readonly AdminAppDbContext _appDbContext;
    private readonly IUserManagerService _userManager;
    public DiagnosisService(AdminAppDbContext appDbContext, IUserManagerService userManager)
    {
        _appDbContext = appDbContext;
        _userManager = userManager;
    }

    public async Task<List<DiagnosisListDTO>> GetDiagnosis()
    {
        var diagnosis = await _appDbContext.DiagnosisTest
            .Where(d => d.IsActive)
            .OrderByDescending(d => d.UpdatedOn ?? d.CreatedOn)
            .Select(d => new DiagnosisListDTO(
                d.Id,
                d.UpdatedOn ?? d.CreatedOn,
                d.Symptoms,
                d.DiagnosisResult,
                d.Prescription
             ))
            .ToListAsync();

        return diagnosis;
    }


    public async Task<DiagnosisDetailsDTO> GetDiagnosisById(int diagnosisId)
    {
        var result = new DiagnosisDetailsDTO();

        var diagnosis = await _appDbContext.DiagnosisTest
            .AsNoTracking()
            .Where(d => d.IsActive && d.Id == diagnosisId)
            .FirstOrDefaultAsync();

        if (diagnosis == null) return result;

        return DiagnosisDetailsDTO.Create(
            diagnosis.DiagnosisResult ?? string.Empty, 
            diagnosis.Description ?? string.Empty, 
            diagnosis.Prescription ?? string.Empty);
    }
    public async Task<DiagnosisDetailsDTO> RunDiagnosis(List<RunDiagnosisDTO> diagnosisRequest)
    {
        try
        {
            var currentUserId = await _userManager.GetUserId();
            var result = new List<DiagnosisResultDTO>();
            var patientSymptomsIds = diagnosisRequest.Select(s => s.SymptomId).ToList();

            var potentialIllnesses = await _appDbContext.Illnesses
                .AsNoTracking()
                .Include(i => i.Rules)
                .Where(i => i.IsActive && i.Rules != null && i.Rules.Any(r => r.IsActive && patientSymptomsIds.Contains(r.SymptomId)))
                .Select(i => new
                {
                    IllnessId = i.Id,
                    i.IllnessName,
                    i.Prescription,
                    i.Description,
                    MatchingRules = i.Rules.Where(r => r.IsActive && patientSymptomsIds.Contains(r.SymptomId)).ToList(),
                    MaxScore = i.Rules.Where(r => r.IsActive).Sum(r => (int)r.Weight)
                })
                .ToListAsync();

            var illnessScores = new Dictionary<int, decimal>();
            var patientSymptomLookup = diagnosisRequest.ToDictionary(d => d.SymptomId, d => d.Duration);

            foreach(var illness in potentialIllnesses)
            {
                decimal currentScore = 0;
                
                foreach(var rule in illness.MatchingRules)
                {
                    if(patientSymptomLookup.TryGetValue(rule.SymptomId, out var patientDuration))
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

                if(currentScore > 0)
                {
                    illnessScores[illness.IllnessId] = currentScore / illness.MaxScore * 100;
                }
            }

            var topIllness = illnessScores
                .OrderByDescending(kvp => kvp.Value)
                .Take(1);

            foreach(var illnessScore in topIllness)
            {
                var illness = potentialIllnesses.FirstOrDefault(i => i.IllnessId == illnessScore.Key);

                if (illness is null) continue;

                if(illness.MaxScore > 0)
                {
                    var percentageToDisplay = illnessScore.Value.ToString("F2");

                    result.Add(new DiagnosisResultDTO(illness.IllnessName, percentageToDisplay));
                }
            }

            var diagnosisResult = new DiagnosisDetailsDTO();

            if (topIllness.Any())
            {
                var top1Illness = topIllness.FirstOrDefault();

                var illness = potentialIllnesses.FirstOrDefault(x => x.IllnessId == top1Illness.Key);

                if(illness is not null)
                {
                    var symptoms = await _appDbContext.Symptoms
                        .AsNoTracking()
                        .Where(s => s.IsActive && patientSymptomsIds.Contains(s.Id))
                        .Select(s => $"{s.Name} ({patientSymptomLookup[s.Id]} Days)")
                        .ToListAsync();

                    var resultIllnesses = result.Select(i => $"{i.Illness} {i.Score}%");
                    var patientDiagnosed = new DiagnosisTestEntity
                    {
                        UserId = currentUserId ?? 0,
                        DiagnosisResult = string.Join(", ", result.Select(i => $"{i.Illness} {i.Score}%")),
                        Symptoms = string.Join(", ", symptoms),
                        Prescription = illness.Prescription,
                        Description = illness.Description,
                        CreatedOn = DateTime.UtcNow,
                        IsActive = true,
                    };

                    await _appDbContext.DiagnosisTest.AddAsync(patientDiagnosed);

                    await _appDbContext.SaveChangesAsync();

                    diagnosisResult = DiagnosisDetailsDTO.Create(
                        patientDiagnosed.DiagnosisResult, 
                        patientDiagnosed.Description ?? string.Empty,
                        patientDiagnosed.Prescription ?? string.Empty);
                }
            }

            return diagnosisResult;
        }
        catch (Exception ex) 
        { 
            Console.WriteLine(ex.ToString());
            throw;
        }
    }
}