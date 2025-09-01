using E_Doctor.Core.Constants.Enums;

namespace E_Doctor.Application.DTOs.Settings.RuleManagements.ExportIllnessDTOs;

public sealed record ExportRulesDTO(
    int SymptomId,
    int IllnessId,
    IllnessRuleConditionEnum Condition,
    int Days,
    IllnessRuleWeightEnum Weight
);