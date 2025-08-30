using E_Doctor.Core.Constants.Enums;
namespace E_Doctor.Application.DTOs.Settings.RuleManagements;

public sealed record IllnessRuleDTO(
    int SymptomId, 
    IllnessRuleConditionEnum Condition,
    int Days,
    IllnessRuleWeightEnum Weight
);