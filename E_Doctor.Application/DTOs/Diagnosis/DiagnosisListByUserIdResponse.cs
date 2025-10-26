namespace E_Doctor.Application.DTOs.Diagnosis;

public sealed record DiagnosisListByUserIdResponse(
    int DiagnosisId,
    DateTime? DiagnoseDate,
    string Symptoms,
    string IllnessName,
    string Prescription
);