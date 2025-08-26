namespace E_Doctor.Application.DTOs.Settings.RuleManagements;

public sealed record IllnessSummaryDTO(
    int IllnessId,
    string IllnessName,
    string Description,
    int RuleCount
    );