namespace E_Doctor.Core.Domain.Entities;
public class DiseaseEntity : BaseEntity
{
    public string DiseaseName { get; set; } = string.Empty;
    public string? Description { get; set; } = string.Empty;
    public ICollection<DiseaseRuleEntity>? Rules { get; set; }
}