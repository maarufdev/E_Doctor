using E_Doctor.Application.DTOs.Common;
using E_Doctor.Core.Constants.Enums;
using System.ComponentModel;
using System.Reflection;

namespace E_Doctor.Application.Helpers
{
    internal static class EnumHelper
    {
        public static List<ConditionRuleDTO> GetDiseaseRuleConditions()
        {
            return Enum.GetValues(typeof(DiseaseRuleConditionEnum))
                .Cast<DiseaseRuleConditionEnum>()
                .Select(e => new ConditionRuleDTO
                {
                    Id = (int)e,
                    ConditionName = GetEnumDescription(e)
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