namespace E_Doctor.Application.DTOs.Settings.RuleManagements;

public sealed record DiseaseDTO(
    int DiseaseId,
    string DiseaseName,
    string Description,
    List<DiseaseRuleDTO> Rules
);