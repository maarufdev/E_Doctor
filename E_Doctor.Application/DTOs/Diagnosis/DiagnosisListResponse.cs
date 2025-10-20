namespace E_Doctor.Application.DTOs.Diagnosis;

public sealed record DiagnosisListResponse(
    int DiagnosisId,
    DateTime? DiagnoseDate,
    string UserName,
    string Symptoms,
    string IllnessName,
    string Prescription);
