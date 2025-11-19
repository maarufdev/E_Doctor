namespace E_Doctor.Application.DTOs.Diagnosis;

public sealed record DiagnosisListResponse(
    int DiagnosisId,
    DateTime? DiagnoseDate,
    string DisplayName,
    string Symptoms,
    string IllnessName,
    string Prescription,
    int? PhysicalExamId);
