namespace E_Doctor.Application.DTOs.Diagnosis.PhysicalExams;
public sealed record PhysicalExamFindingsRequest(
    int PhysicalItemId,
    bool IsNormal,
    string NormalDescriptions,
    string AbnormalFindings
    );
