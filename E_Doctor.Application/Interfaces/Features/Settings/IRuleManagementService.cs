using E_Doctor.Application.DTOs.Settings.RuleManagements;
namespace E_Doctor.Application.Interfaces.Features.Settings;

public interface IRuleManagementService
{
    Task<IEnumerable<DiseaseSummaryDTO>> GetDiseaseList();
    Task<DiseaseDTO> GetDiseaseRule(int id);
    Task<bool> SaveDisease(DiseaseDTO requestDto);
    Task<bool> DeleteDisease(int id);
}
