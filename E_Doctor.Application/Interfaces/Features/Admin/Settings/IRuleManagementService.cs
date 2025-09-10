using E_Doctor.Application.DTOs.Common.ExportIllnessDTOs;
using E_Doctor.Application.DTOs.Settings.RuleManagements;
using E_Doctor.Application.DTOs.Settings.Symptoms;
namespace E_Doctor.Application.Interfaces.Features.Admin.Settings;

public interface IRuleManagementService
{
    Task<IEnumerable<IllnessSummaryDTO>> GetIllnessList();
    Task<IllnessDTO> GetIllnessById(int id);
    Task<bool> SaveIllness(IllnessDTO requestDto);
    Task<bool> DeleteIllnessById(int id);
    Task<byte[]> ExportRulesConfigration();
    Task<IEnumerable<GetSymptomDTO>> GetSymptoms();
}
