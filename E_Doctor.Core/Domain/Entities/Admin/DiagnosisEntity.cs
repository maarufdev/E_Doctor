namespace E_Doctor.Core.Domain.Entities.Admin;
public class DiagnosisEntity : BaseEntity
{
    public int UserId { get; set; }
    public ICollection<DiagnosisIllnessesEntity>? DiagnosIllnesses { get; set; } = new List<DiagnosisIllnessesEntity>();
    public ICollection<DiagnosisSymptomsEntity>? DiagnosSymptoms { get; set; } = new List<DiagnosisSymptomsEntity>();
}