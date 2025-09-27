using E_Doctor.Core.Constants.Enums;

namespace E_Doctor.Core.Domain.Entities.Patient;
public class PatientRulesEntity
{
    public int SymptomId { get; set; }
    public PatientSymptomEntity? Symptom { get; set; }
    public int IllnessId { get; set; }
    public PatientIllnessEntity? Illness { get; set; }
    public string Question { get; set; } = string.Empty;
}

//public class PatientRulesEntity
//{
//    public int SymptomId { get; set; }
//    public PatientSymptomEntity? Symptom { get; set; }
//    public int IllnessId { get; set; }
//    public PatientIllnessEntity? Illness { get; set; }
//    public IllnessRuleConditionEnum Condition { get; set; }
//    public int Days { get; set; }
//    public IllnessRuleWeightEnum Weight { get; set; }
//}