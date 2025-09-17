namespace E_Doctor.Application.DTOs.Settings.RuleManagements;
public sealed record IllnessDTO(
    int IllnessId,
    string IllnessName,
    string Description,
    string Prescription,
    List<IllnessRuleDTO> Rules
);