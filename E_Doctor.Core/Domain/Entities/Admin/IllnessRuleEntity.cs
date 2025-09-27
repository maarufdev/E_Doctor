using E_Doctor.Core.Constants.Enums;

namespace E_Doctor.Core.Domain.Entities.Admin;
public class IllnessRuleEntity
{
    public int SymptomId { get; set; }
    public virtual SymptomEntity? Symptom { get; set; }
    public int IllnessId { get; set; }
    public virtual IllnessEntity? Illness { get; set; }
    public string Question { get; set; } = string.Empty;
    public bool IsActive { get; set; }
}

//public class IllnessRuleEntity
//{
//    public int SymptomId { get; set; }
//    public virtual SymptomEntity? Symptom { get; set; }
//    public int IllnessId { get; set; }
//    public virtual IllnessEntity? Illness { get; set; }
//    public IllnessRuleConditionEnum Condition { get; set; }
//    public int Days { get; set; }
//    public IllnessRuleWeightEnum Weight { get; set; }
//    public bool IsActive { get; set; }
//}