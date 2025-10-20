using E_Doctor.Application.DTOs.Common;
using E_Doctor.Application.Helpers;
using E_Doctor.Application.Interfaces.Features.Common;

namespace E_Doctor.Application.Services.Features.Common
{
    internal class CommonService : ICommonService
    {
        public IEnumerable<ConditionRuleDTO> GetRuleConditions()
        {
            return EnumHelper.GetIllnessRuleConditions();
        }

        public IEnumerable<WeightRuleDTO> GetWeightRules()
        {
            return EnumHelper.GetIllnessRuleWeights();
        }
    }
}