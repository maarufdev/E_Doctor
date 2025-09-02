using E_Doctor.Application.DTOs.Diagnosis;
using E_Doctor.Application.DTOs.Settings.Symptoms;

namespace E_Doctor.Application.Interfaces.Features.Patient.Diagnosis;
public interface IPatientService
{
    Task<bool> MigrateRules(string jsonData);
    Task<IEnumerable<GetSymptomDTO>> GetSymtoms();
    Task<List<DiagnosisResultDTO>> RunDiagnosis(List<RunDiagnosisDTO> diagnosisRequest);
    Task<List<DiagnosisListDTO>> GetDiagnosis();
}
