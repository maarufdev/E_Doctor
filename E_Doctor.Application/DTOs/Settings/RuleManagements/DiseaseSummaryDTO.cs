namespace E_Doctor.Application.DTOs.Settings.RuleManagements;

public sealed record DiseaseSummaryDTO(
    int DiseaseId,
    string DiseaseName,
    string Description,
    int RuleCount
    );