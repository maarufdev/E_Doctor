using System.ComponentModel;

namespace E_Doctor.Core.Constants.Enums;
public enum DiseaseRuleConditionEnum
{
    [Description("IsEqual")]
    IsEqual = 1,

    [Description("IsMoreThan")]
    IsMoreThan = 2,

    [Description("IsLessThan")]
    IsLessThan = 3
}
