using E_Doctor.Application.DTOs.Common;
using E_Doctor.Application.DTOs.Common.CustomResultDTOs;
using E_Doctor.Application.DTOs.Diagnosis;

namespace E_Doctor.Application.Interfaces.Features.Diagnosis;
public interface IDiagnosisService
{
    Task<DiagnosisDetailsDTO> GetDiagnosisById(int diagnosisId);
    Task<Result<DiagnosisDetailsDTO>> RunDiagnosis(RunDiagnosisDTO diagnosisRequest);
    Task<PagedResult<DiagnosisListResponse>> GetDiagnosis(GetDiagnosisParamsDTO requestParams);
    Task<PagedResult<DiagnosisListByUserIdResponse>> GetDiagnosisByUserId(GetDiagnosisParamsDTO requestParams);
    Task<List<GetConsultationIllnessDTO>> GetConsultationIllnessList();
    Task<Result<List<GetConsultationSymptomByIllnessIdDTO>>> GetConsultationSymptomByIllnessId(int illnessId);
    Task<Result> DeleteDiagnosisById(int diagnosisId);
}