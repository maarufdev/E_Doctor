namespace E_Doctor.Core.Domain.Entities;
public class IllnessEntity : BaseEntity
{
    public string IllnessName { get; set; } = string.Empty;
    public string? Description { get; set; } = string.Empty;
    public ICollection<IllnessRuleEntity>? Rules { get; set; }
}