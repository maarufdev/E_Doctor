using E_Doctor.Application.DTOs.Common;
using E_Doctor.Core.Constants.Enums;
using System.ComponentModel;
using System.Reflection;

namespace E_Doctor.Application.Helpers
{
    public static class EnumHelper
    {
        public static List<ConditionRuleDTO> GetIllnessRuleConditions()
        {
            return Enum.GetValues(typeof(IllnessRuleConditionEnum))
                .Cast<IllnessRuleConditionEnum>()
                .Select(e => new ConditionRuleDTO
                {
                    Id = (int)e,
                    ConditionName = GetEnumDescription(e)
                })
                .ToList();
        }

        public static List<WeightRuleDTO> GetIllnessRuleWeights()
        {
            return Enum.GetValues(typeof(IllnessRuleWeightEnum))
                .Cast<IllnessRuleWeightEnum>()
                .Select(e => new WeightRuleDTO
                {
                    Id = (int)e,
                    Name = GetEnumDescription(e)
                })
                .ToList();
        }

        private static string GetEnumDescription(Enum value)
        {
            var field = value.GetType().GetField(value.ToString());
            var attribute = field?.GetCustomAttribute<DescriptionAttribute>();
            return attribute?.Description ?? value.ToString();
        }

    }
}