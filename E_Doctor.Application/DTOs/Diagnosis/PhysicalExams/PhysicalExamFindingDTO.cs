namespace E_Doctor.Application.DTOs.Diagnosis.PhysicalExams;
public sealed record PhysicalExamFindingDTO
{
    public int PhysicalExamId { get; set; }
    public int PhysicalItemId { get; set; }
    public bool IsNormal { get; set; }
    public string? NormalDescription { get; set; } = string.Empty;
    public string? AbnormalFindings { get; set; } = string.Empty;
}
