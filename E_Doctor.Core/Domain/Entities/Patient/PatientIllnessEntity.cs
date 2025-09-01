namespace E_Doctor.Core.Domain.Entities.Patient;

public class PatientIllnessEntity
{
    public int IllnessId { get; set; }
    public string IllnessName { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public DateTime UpdatedOn { get; set; }
}