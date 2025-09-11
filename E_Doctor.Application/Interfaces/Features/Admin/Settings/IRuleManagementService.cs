using E_Doctor.Application.DTOs.Common;
using E_Doctor.Application.DTOs.Common.CustomResultDTOs;
using E_Doctor.Application.DTOs.Settings.RuleManagements;
using E_Doctor.Application.DTOs.Settings.Symptoms;
namespace E_Doctor.Application.Interfaces.Features.Admin.Settings;

public interface IRuleManagementService
{
    Task<PagedResult<IllnessSummaryDTO>> GetIllnessList(GetIllnessRequestDTO requestParam);
    Task<IllnessDTO> GetIllnessById(int id);
    Task<Result> SaveIllness(IllnessDTO requestDto);
    Task<bool> DeleteIllnessById(int id);
    Task<byte[]> ExportRulesConfigration();
    Task<IEnumerable<GetSymptomDTO>> GetSymptoms();
}
