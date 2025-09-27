using E_Doctor.Application.DTOs.Common;
using E_Doctor.Application.DTOs.Common.CustomResultDTOs;
using E_Doctor.Application.DTOs.Diagnosis;
using E_Doctor.Application.DTOs.Settings.Symptoms;

namespace E_Doctor.Application.Interfaces.Features.Patient.Diagnosis;
public interface IPatientService
{
    Task<bool> MigrateRules(string jsonData);
    Task<IEnumerable<GetSymptomDTO>> GetSymtoms();
    Task<DiagnosisDetailsDTO> GetDiagnosisById(int diagnosisId);
    Task<Result<DiagnosisDetailsDTO>> RunDiagnosis(RunDiagnosisDTO diagnosisRequest);
    Task<List<DiagnosisListDTO>> GetDiagnosis();
    Task<List<GetConsultationIllnessDTO>> GetConsultationIllnessList();
    Task<Result<List<GetConsultationSymptomByIllnessIdDTO>>> GetConsultationSymptomByIllnessId(int illnessId);
}
