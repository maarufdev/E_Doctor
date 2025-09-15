namespace E_Doctor.Core.Domain.Entities.Admin;
public class DiagnosisIllnessesEntity
{
    public int Id { get; set; }
    public int DiagnosisId { get; set; }
    public DiagnosisEntity? Diagnosis { get; set; }
    public int SymptomId { get; set; }
    public virtual SymptomEntity? Symptom { get; set; }
    public int Days { get; set; }
}