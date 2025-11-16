namespace E_Doctor.Core.Domain.Entities.Patient.PhysicalExam;
public class PhysicalExamItemsEntity : BaseEntity
{
    public string Label { get; set; } = string.Empty;
    public string? InputType { get; set; } = string.Empty;
    public int SortOrder { get; set; }
}