namespace E_Doctor.Application.DTOs.Settings.RuleManagements;
public sealed record IllnessDTO(
    int IllnessId,
    string IllnessName,
    string Description,
    string Prescription,
    string Notes,
    List<IllnessRuleDTO> Rules
);