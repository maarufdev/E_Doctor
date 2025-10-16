namespace E_Doctor.Core.Domain.Entities.Admin
{
    public class SymptomEntity : BaseEntity
    {
        public string Name { get; set; } = string.Empty;
        public string QuestionText { get; set; } = string.Empty;
        public ICollection<IllnessRuleEntity>? Rules { get; set; }
    }
}