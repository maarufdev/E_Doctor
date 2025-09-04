namespace E_Doctor.Core.Domain.Entities.Patient;
public class PatientDiagnosisEntity : BaseEntity
{
    public ICollection<PatientDiagnosisIllnessEntity>? DiagnosIllnesses { get; set; }
    public ICollection<PatientDiagnosisSymptomEntity>? DiagnosSymptoms { get; set; }
    public int? UserId { get; set; }
}