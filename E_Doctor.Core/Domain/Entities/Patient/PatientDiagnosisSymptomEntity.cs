namespace E_Doctor.Core.Domain.Entities.Patient;
public class PatientDiagnosisSymptomEntity : BaseEntity
{
    public int DiagnosisId { get; set; }
    public PatientDiagnosisEntity? Diagnosis { get; set; }
    public string? SymptomName { get; set; }
    public int Days { get; set; }
}