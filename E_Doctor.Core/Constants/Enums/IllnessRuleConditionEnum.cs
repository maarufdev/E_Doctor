using System.ComponentModel;

namespace E_Doctor.Core.Constants.Enums;
public enum IllnessRuleConditionEnum
{
    [Description("IsEqual")]
    IsEqual,

    [Description("IsLessThan")]
    IsLessThan,
    
    [Description("IsLessThanOrEqual")]
    IsLessThanOrEqual,
    
    [Description("IsMoreThan")]
    IsMoreThan,
    
    [Description("IsMoreThanOrEqual")]
    IsMoreThanOrEqual,
}
