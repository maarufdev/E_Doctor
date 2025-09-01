using E_Doctor.Application.DTOs.Diagnosis;
using E_Doctor.Application.Interfaces.Features.Admin.Diagnosis;
using E_Doctor.Core.Constants.Enums;
using E_Doctor.Core.Domain.Entities.Admin;
using E_Doctor.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace E_Doctor.Application.Services.Admin.Diagnosis;

internal class DiagnosisService : IDiagnosisService
{
    private readonly AdminAppDbContext _appDbContext;
    public DiagnosisService(AdminAppDbContext appDbContext)
    {
        _appDbContext = appDbContext;
    }

    public async Task<List<DiagnosisListDTO>> GetDiagnosis()
    {
        var diagnosis = await _appDbContext.Diagnosis
            .Where(d => d.IsActive)
            .OrderByDescending(d => d.UpdatedOn ?? d.CreatedOn)
            .Select(d => new DiagnosisListDTO(
                d.Id,
                d.UpdatedOn ?? d.CreatedOn,
                d.Symptoms,
                d.IllnessName
             ))
            .ToListAsync();

        return diagnosis;
    }

    public async Task<List<DiagnosisResultDTO>> RunDiagnosis(List<RunDiagnosisDTO> diagnosisRequest)
    {
        try
        {
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
                    MatchingRules = i.Rules.Where(r => r.IsActive && patientSymptomsIds.Contains(r.SymptomId)).ToList(),
                    MaxScore = i.Rules.Where(r => r.IsActive).Sum(r => (int)r.Weight)
                })
                .ToListAsync();

            var illnessScores = new Dictionary<int, double>();
            var patientSymptomLookup = diagnosisRequest.ToDictionary(d => d.SymptomId, d => d.Duration);

            foreach(var illness in potentialIllnesses)
            {
                double currentScore = 0;
                
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
                    illnessScores[illness.IllnessId] = currentScore;
                }
            }

            var top5Illnesses = illnessScores
                .OrderByDescending(kvp => kvp.Value)
                .Take(5);

            foreach(var illnessScore in top5Illnesses)
            {
                var illness = potentialIllnesses.FirstOrDefault(i => i.IllnessId == illnessScore.Key);

                if (illness is null) continue;

                if(illness.MaxScore > 0)
                {
                    var percentage = illnessScore.Value / illness.MaxScore * 100;
                    var percentageToDisplay = percentage.ToString("F2");

                    result.Add(new DiagnosisResultDTO(illness.IllnessName, percentageToDisplay));
                }
            }

            if (top5Illnesses.Any())
            {
                var top1Illness = top5Illnesses.FirstOrDefault();

                var illness = potentialIllnesses.FirstOrDefault(x => x.IllnessId == top1Illness.Key);

                if(illness is not null)
                {
                    var symptoms = await _appDbContext.Symptoms
                        .AsNoTracking()
                        .Where(s => s.IsActive && patientSymptomsIds.Contains(s.Id))
                        .Select(s => $"{s.Name} ({patientSymptomLookup[s.Id]} Days)")
                        .ToListAsync();

                    var patientDiagnosed = new DiagnosisEntity
                    {
                        IllnessName = $"{result[0].Illness} {result[0].Score}%",
                        Symptoms = string.Join(",", symptoms),
                        CreatedOn = DateTime.UtcNow,
                        IsActive = true,
                    };

                    await _appDbContext.Diagnosis.AddAsync(patientDiagnosed);

                    await _appDbContext.SaveChangesAsync();
                }
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