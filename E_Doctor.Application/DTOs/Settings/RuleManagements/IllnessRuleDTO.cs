namespace E_Doctor.Application.DTOs.Settings.RuleManagements;

public sealed record IllnessRuleDTO(
    int SymptomId,
    string Question
);

//public sealed record IllnessRuleDTO(
//    int SymptomId,
//    IllnessRuleConditionEnum Condition,
//    int Days,
//    IllnessRuleWeightEnum Weight
//);