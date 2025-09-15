namespace E_Doctor.Core.Domain.Entities.Admin;

public class DiagnosisTestEntity : BaseEntity
{
    public int UserId { get; set; }
    public string? DiagnosisResult { get; set; }
    public string? Symptoms { get; set; }
    public string? Prescription { get; set; }
}
