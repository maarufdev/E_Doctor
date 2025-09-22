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
    //public async Task<DiagnosisDetailsDTO> RunDiagnosis(List<RunDiagnosisDTO> diagnosisRequest)
    //{
    //    try
    //    {
    //        var currentUserId = await _userManager.GetUserId();
    //        var result = new List<DiagnosisResultDTO>();
    //        var patientSymptomsIds = diagnosisRequest.Select(s => s.SymptomId).ToList();

    //        var potentialIllnesses = await _appDbContext.Illnesses
    //            .AsNoTracking()
    //            .Include(i => i.Rules)
    //            .Where(i => i.IsActive && i.Rules != null && i.Rules.Any(r => r.IsActive && patientSymptomsIds.Contains(r.SymptomId)))
    //            .Select(i => new
    //            {
    //                IllnessId = i.Id,
    //                i.IllnessName,
    //                i.Prescription,
    //                i.Description,
    //                MatchingRules = i.Rules.Where(r => r.IsActive && patientSymptomsIds.Contains(r.SymptomId)).ToList(),
    //                MaxScore = i.Rules.Where(r => r.IsActive).Sum(r => (int)r.Weight)
    //            })
    //            .ToListAsync();

    //        var illnessScores = new Dictionary<int, decimal>();
    //        var patientSymptomLookup = diagnosisRequest.ToDictionary(d => d.SymptomId, d => d.Duration);

    //        foreach(var illness in potentialIllnesses)
    //        {
    //            decimal currentScore = 0;

    //            foreach(var rule in illness.MatchingRules)
    //            {
    //                if(patientSymptomLookup.TryGetValue(rule.SymptomId, out var patientDuration))
    //                {
    //                    bool isRuleMet = false;

    //                    switch (rule.Condition)
    //                    {
    //                        case IllnessRuleConditionEnum.IsEqual:
    //                            isRuleMet = patientDuration == rule.Days;
    //                            break;

    //                        case IllnessRuleConditionEnum.IsLessThan:
    //                            isRuleMet = patientDuration < rule.Days;
    //                            break;

    //                        case IllnessRuleConditionEnum.IsLessThanOrEqual:
    //                            isRuleMet = patientDuration <= rule.Days;
    //                            break;

    //                        case IllnessRuleConditionEnum.IsMoreThan:
    //                            isRuleMet = patientDuration > rule.Days;
    //                            break;

    //                        case IllnessRuleConditionEnum.IsMoreThanOrEqual:
    //                            isRuleMet = patientDuration >= rule.Days;
    //                            break;
    //                    }

    //                    if (isRuleMet)
    //                    {
    //                        currentScore += (int)rule.Weight;
    //                    }
    //                }
    //            }

    //            if(currentScore > 0)
    //            {
    //                illnessScores[illness.IllnessId] = currentScore / illness.MaxScore * 100;
    //            }
    //        }

    //        var topIllness = illnessScores
    //            .OrderByDescending(kvp => kvp.Value)
    //            .Take(1);

    //        foreach(var illnessScore in topIllness)
    //        {
    //            var illness = potentialIllnesses.FirstOrDefault(i => i.IllnessId == illnessScore.Key);

    //            if (illness is null) continue;

    //            if(illness.MaxScore > 0)
    //            {
    //                var percentageToDisplay = illnessScore.Value.ToString("F2");

    //                result.Add(new DiagnosisResultDTO(illness.IllnessName, percentageToDisplay));
    //            }
    //        }

    //        var diagnosisResult = new DiagnosisDetailsDTO();

    //        if (topIllness.Any())
    //        {
    //            var top1Illness = topIllness.FirstOrDefault();

    //            var illness = potentialIllnesses.FirstOrDefault(x => x.IllnessId == top1Illness.Key);

    //            if(illness is not null)
    //            {
    //                var symptoms = await _appDbContext.Symptoms
    //                    .AsNoTracking()
    //                    .Where(s => s.IsActive && patientSymptomsIds.Contains(s.Id))
    //                    .Select(s => $"{s.Name} ({patientSymptomLookup[s.Id]} Days)")
    //                    .ToListAsync();

    //                var resultIllnesses = result.Select(i => $"{i.Illness} {i.Score}%");
    //                var patientDiagnosed = new DiagnosisTestEntity
    //                {
    //                    UserId = currentUserId ?? 0,
    //                    DiagnosisResult = string.Join(", ", result.Select(i => $"{i.Illness} {i.Score}%")),
    //                    Symptoms = string.Join(", ", symptoms),
    //                    Prescription = illness.Prescription,
    //                    Description = illness.Description,
    //                    CreatedOn = DateTime.UtcNow,
    //                    IsActive = true,
    //                };

    //                await _appDbContext.DiagnosisTest.AddAsync(patientDiagnosed);

    //                await _appDbContext.SaveChangesAsync();

    //                diagnosisResult = DiagnosisDetailsDTO.Create(
    //                    patientDiagnosed.DiagnosisResult, 
    //                    patientDiagnosed.Description ?? string.Empty,
    //                    patientDiagnosed.Prescription ?? string.Empty);
    //            }
    //        }

    //        return diagnosisResult;
    //    }
    //    catch (Exception ex) 
    //    { 
    //        Console.WriteLine(ex.ToString());
    //        throw;
    //    }
    //}

    public async Task<DiagnosisDetailsDTO> RunDiagnosis(List<RunDiagnosisDTO> diagnosisRequest)
    {
        var currentUserId = await _userManager.GetUserId();

        // 1. GATHER KNOWN FACTS: The patient's reported symptoms are our facts.
        var patientSymptomFacts = diagnosisRequest.ToDictionary(d => d.SymptomId, d => d.Duration);

        // 2. GET ALL POSSIBLE GOALS: Each active illness is a potential goal or "hypothesis".
        var allHypotheses = await _appDbContext.Illnesses
            .AsNoTracking()
            .Include(i => i.Rules)
            .Where(i => i.IsActive && i.Rules != null && i.Rules.Any(r => r.IsActive))
            .ToListAsync();

        var confirmedHypothesesScores = new Dictionary<int, decimal>();

        // 3. ITERATE THROUGH EACH GOAL (HYPOTHESIS) to see if it can be proven.
        foreach (var illnessHypothesis in allHypotheses)
        {
            // For each goal, try to verify it using the known facts.
            decimal confidenceScore = EvaluateHypothesis(illnessHypothesis, patientSymptomFacts);

            if (confidenceScore > 0)
            {
                confirmedHypothesesScores[illnessHypothesis.Id] = confidenceScore;
            }
        }

        // 4. SELECT THE BEST CONCLUSION: Find the hypothesis with the highest confidence score.
        if (!confirmedHypothesesScores.Any())
        {
            return new DiagnosisDetailsDTO(); // No hypothesis could be supported by the facts.
        }

        var topHypothesis = confirmedHypothesesScores.OrderByDescending(x => x.Value).First();
        var confirmedIllness = allHypotheses.First(i => i.Id == topHypothesis.Key);

        // --- The rest of the logic for saving and returning the result remains the same ---
        // (Assuming DTO creation and DB saving logic is correct)
        // ... (Saving and returning logic would go here) ...

        return new DiagnosisDetailsDTO(); // Placeholder for your DTO creation
    }

    /// <summary>
    /// Evaluates a single hypothesis (an illness) by working backward from its required rules
    /// to see if they are supported by the patient's symptoms (the facts).
    /// </summary>
    /// <param name="illnessHypothesis">The goal to be proven.</param>
    /// <param name="patientSymptomFacts">The known facts from the patient.</param>
    /// <returns>A confidence score (0-100) for the hypothesis.</returns>
    private decimal EvaluateHypothesis(IllnessEntity illnessHypothesis, Dictionary<int, int> patientSymptomFacts)
    {
        // These are the conditions that MUST be met to prove our hypothesis.
        var requiredRules = illnessHypothesis.Rules.Where(r => r.IsActive).ToList();

        if (!requiredRules.Any())
        {
            return 0;
        }

        decimal currentScore = 0;
        decimal maxScore = requiredRules.Sum(r => (int)r.Weight);

        // For each required rule, check if there is a corresponding fact to support it.
        foreach (var rule in requiredRules)
        {
            // Check the fact base for evidence (a matching symptom).
            if (patientSymptomFacts.TryGetValue(rule.SymptomId, out var patientDuration))
            {
                // We found a relevant fact. Now, verify if it satisfies the rule's condition.
                bool isRuleSatisfied = false;
                switch (rule.Condition)
                {
                    case IllnessRuleConditionEnum.IsEqual:
                        isRuleSatisfied = patientDuration == rule.Days;
                        break;
                    case IllnessRuleConditionEnum.IsLessThan:
                        isRuleSatisfied = patientDuration < rule.Days;
                        break;
                    case IllnessRuleConditionEnum.IsLessThanOrEqual:
                        isRuleSatisfied = patientDuration <= rule.Days;
                        break;
                    case IllnessRuleConditionEnum.IsMoreThan:
                        isRuleSatisfied = patientDuration > rule.Days;
                        break;
                    case IllnessRuleConditionEnum.IsMoreThanOrEqual:
                        isRuleSatisfied = patientDuration >= rule.Days;
                        break;
                }

                if (isRuleSatisfied)
                {
                    // This piece of evidence supports our hypothesis.
                    currentScore += (int)rule.Weight;
                }
            }
        }

        if (maxScore > 0)
        {
            // Return the final confidence score for this single hypothesis.
            return (currentScore / maxScore) * 100;
        }

        return 0;
    }

}