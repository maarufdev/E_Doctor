using E_Doctor.Application.DTOs.Settings.Symptoms;

namespace E_Doctor.Application.Interfaces.Features.Settings;
public interface ISymptomService
{
    Task<IEnumerable<GetSymptomDTO>> GetSymptoms();
    Task<GetSymptomDTO?> GetSymptomById(int symptomId);
    Task<bool> SaveSymptom(SaveSymptomDTO saveSymptomDto);
    Task<bool> RemoveSymptom(int symptomId);
}