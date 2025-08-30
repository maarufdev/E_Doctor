using E_Doctor.Application.DTOs.Settings.RuleManagements;
namespace E_Doctor.Application.Interfaces.Features.Settings;

public interface IRuleManagementService
{
    Task<IEnumerable<IllnessSummaryDTO>> GetIllnessList();
    Task<IllnessDTO> GetIllnessById(int id);
    Task<bool> SaveIllness(IllnessDTO requestDto);
    Task<bool> DeleteIllnessById(int id);
}
