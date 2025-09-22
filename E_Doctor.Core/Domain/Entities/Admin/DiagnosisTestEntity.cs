namespace E_Doctor.Core.Domain.Entities.Admin;

public class DiagnosisTestEntity : BaseEntity
{
    public int UserId { get; set; }
    public string? DiagnosisResult { get; set; } = string.Empty;
    public string? Symptoms { get; set; } = string.Empty;
    public string? Prescription { get; set; } = string.Empty;
    public string? Description { get; set; } = string.Empty;
    public string? Notes { get; set; } = string.Empty;
}
