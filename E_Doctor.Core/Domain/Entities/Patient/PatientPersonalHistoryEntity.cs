namespace E_Doctor.Core.Domain.Entities.Patient;
public class PatientPersonalHistoryEntity : BaseEntity
{
    public int PatientInfoId { get; set; }
    public PatientInformationEntity? PatientInformation { get; set; }
    public bool Smoker { get; set; }
    public bool AlchoholBeverageDrinker { get; set; }
    public bool IllicitDrugUser { get; set; }
    public string? Others { get; set; } = string.Empty;
}