namespace E_Doctor.Core.Domain.Entities.Patient.PhysicalExam;

public sealed class PhysicalExamEntity : BaseEntity
{
    public int DiagnosisId { get; set; }
    public string BP { get; set; } = string.Empty;
    public string HR { get; set; } = string.Empty;
    public string RR { get; set; } = string.Empty;
    public string Temp { get; set; } = string.Empty;
    public string Weight { get; set; } = string.Empty;
    public string O2Sat { get; set; } = string.Empty;
    public ICollection<PhysicalExamFindingsEntity>? PhysicalExamFindings { get; set; }
}
