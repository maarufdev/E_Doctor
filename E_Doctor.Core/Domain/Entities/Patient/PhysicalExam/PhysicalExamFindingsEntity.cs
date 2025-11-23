namespace E_Doctor.Core.Domain.Entities.Patient.PhysicalExam;
public sealed class PhysicalExamFindingsEntity : BaseEntity
{
    public int PhysicalExamId { get; set; }
    public int PhysicalItemId { get; set; }
    public bool IsNormal { get; set; }
    public string? NormalDescription { get; set; } = string.Empty;
    public string? AbnormalFindings { get; set; } = string.Empty;

    // Navigation Properties
    public PhysicalExamEntity? PhysicalExam { get; set; }
    public PhysicalExamItemsEntity? PhysicalExamItem { get; set; }
}