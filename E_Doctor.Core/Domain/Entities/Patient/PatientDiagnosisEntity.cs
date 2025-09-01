namespace E_Doctor.Core.Domain.Entities.Patient;
public class PatientDiagnosisEntity : BaseEntity
{
    public string? IllnessName { get; set; }
    public string? Symptoms { get; set; }
}
