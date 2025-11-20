namespace E_Doctor.Application.DTOs.Diagnosis.PhysicalExams;
public sealed record SavePhysicalExamRequest(
    int DiagnosisId,
    int PhysicalExamId,
    string BP,
    string HR,
    string RR,
    string Temp,
    string Weight,
    string O2Sat,
    List<PhysicalExamFindingsRequest> PhysicalExamFindings
);

