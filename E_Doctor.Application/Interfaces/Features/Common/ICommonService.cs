using E_Doctor.Application.DTOs.Common;

namespace E_Doctor.Application.Interfaces.Features.Common;
public interface ICommonService
{
    IEnumerable<ConditionRuleDTO> GetRuleConditions();
    IEnumerable<WeightRuleDTO> GetWeightRules();
}
