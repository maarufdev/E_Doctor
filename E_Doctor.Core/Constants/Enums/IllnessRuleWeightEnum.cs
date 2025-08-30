using System.ComponentModel;
namespace E_Doctor.Core.Constants.Enums;

public enum IllnessRuleWeightEnum
{
    [Description("Low")]
    Low = 3,
    
    [Description("Medium")]
    Medium = 6,

    [Description("High")]
    High = 9,
}