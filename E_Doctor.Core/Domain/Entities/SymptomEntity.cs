namespace E_Doctor.Core.Domain.Entities
{
    public class SymptomEntity : BaseEntity
    {
        public string Name { get; set; } = string.Empty;
        public ICollection<IllnessRuleEntity>? Rules { get; set; }
    }
}