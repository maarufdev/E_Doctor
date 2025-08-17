using E_Doctor.Core.Constants.Enums;
namespace E_Doctor.Application.DTOs.Settings.RuleManagements;

public sealed record DiseaseRuleDTO(
    int SymptomId, 
    DiseaseRuleConditionEnum Condition,
    int Days
);