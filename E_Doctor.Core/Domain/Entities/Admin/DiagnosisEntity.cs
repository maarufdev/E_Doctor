namespace E_Doctor.Core.Domain.Entities.Admin;

public class DiagnosisEntity : BaseEntity
{
    public int UserId { get; set; }

    //public ICollection<DiagnosisSymptomsEntity>? DiagnosisSymptoms { get; set; } = [];
    //public ICollection<DiagnosisIllnessesEntity>? DiagnosisIllnesses { get; set; } = [];

    // TODO: Instead of IllnessName, this should be the result
    public string? IllnessName { get; set; }
    public string? Symptoms { get; set; }

    // TODO: add the prescription here for testing.
    //public string? Prescription { get; set; }
}
