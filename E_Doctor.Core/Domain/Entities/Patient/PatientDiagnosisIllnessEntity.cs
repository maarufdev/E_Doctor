namespace E_Doctor.Core.Domain.Entities.Patient;

public class PatientDiagnosisIllnessEntity : BaseEntity
{
    public int DiagnosisId { get; set; }
    public PatientDiagnosisEntity? Diagnosis { get; set; }
    public string Illness { get; set; } = string.Empty;
    public string Prescription { get; set; } = string.Empty;
    public decimal Score { get; set; }
}
