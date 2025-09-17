using E_Doctor.Application.DTOs.Diagnosis;

namespace E_Doctor.Application.Interfaces.Features.Admin.Diagnosis;
public interface IDiagnosisService
{
    Task<DiagnosisDetailsDTO> GetDiagnosisById(int diagnosisId);
    Task<DiagnosisDetailsDTO> RunDiagnosis(List<RunDiagnosisDTO> diagnosisRequest);
    Task<List<DiagnosisListDTO>> GetDiagnosis();
}