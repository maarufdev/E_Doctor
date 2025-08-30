namespace E_Doctor.Application.DTOs.Diagnosis;

public sealed record DiagnosisListDTO(
    int DiagnosisId,
    DateTime? DiagnoseDate,
    string Symptoms,
    string IllnessName
);