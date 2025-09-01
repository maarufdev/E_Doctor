using E_Doctor.Application.DTOs.Settings.RuleManagements;
using E_Doctor.Application.DTOs.Settings.RuleManagements.ExportIllnessDTOs;
namespace E_Doctor.Application.Interfaces.Features.Settings;

public interface IRuleManagementService
{
    Task<IEnumerable<IllnessSummaryDTO>> GetIllnessList();
    Task<IllnessDTO> GetIllnessById(int id);
    Task<bool> SaveIllness(IllnessDTO requestDto);
    Task<bool> DeleteIllnessById(int id);
    Task<byte[]> ExportRulesConfigration();
}
