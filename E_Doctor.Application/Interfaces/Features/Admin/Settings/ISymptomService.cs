using E_Doctor.Application.DTOs.Common;
using E_Doctor.Application.DTOs.Common.CustomResultDTOs;
using E_Doctor.Application.DTOs.Settings.Symptoms;

namespace E_Doctor.Application.Interfaces.Features.Admin.Settings;
public interface ISymptomService
{
    Task<PagedResult<GetSymptomDTO>> GetSymptoms(GetSymptomsRequestDTO requestDTO);
    Task<GetSymptomDTO?> GetSymptomById(int symptomId);
    Task<Result> SaveSymptom(SaveSymptomDTO saveSymptomDto);
    Task<bool> RemoveSymptom(int symptomId);
}