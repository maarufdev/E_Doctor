namespace E_Doctor.Core.Domain.Entities.Patient;
public class PatientFamilyHistoryEntity : BaseEntity
{
    public int PatientInfoId { get; set; }
    public PatientInformationEntity? PatientInformation { get; set; }
    public bool PTB { get; set; }
    public bool Hypertension { get; set; }
    public bool Cardiac { get; set; }
    public bool None { get; set; }
    public bool Diabetes { get; set; }
    public bool Asthma { get; set; }
    public bool Cancer { get; set; }
    public string? Others { get; set; } = string.Empty;
}
