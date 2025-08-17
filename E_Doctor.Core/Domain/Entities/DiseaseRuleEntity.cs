using E_Doctor.Core.Constants.Enums;

namespace E_Doctor.Core.Domain.Entities;
public class DiseaseRuleEntity
{
    public int SymptomId { get; set; }
    public virtual SymptomEntity? Symptom { get; set; }
    public int DiseaseId { get; set; }
    public virtual DiseaseEntity? Disease { get; set; }
    public DiseaseRuleConditionEnum Condition { get; set; }
    public int Days { get; set; }
    public bool IsActive { get; set; }
}