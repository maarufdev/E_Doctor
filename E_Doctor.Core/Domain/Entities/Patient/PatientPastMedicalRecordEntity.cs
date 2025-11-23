namespace E_Doctor.Core.Domain.Entities.Patient;
public class PatientPastMedicalRecordEntity : BaseEntity
{
    public int PatientInfoId { get; set; }
    public PatientInformationEntity? PatientInformation { get; set; }
    public bool PreviousHospitalization { get; set; }
    public bool PastSurgery { get; set; }
    public bool Diabetes { get; set; }
    public bool Hypertension { get; set; }
    public bool AllergyToMeds { get; set; }
    public bool HeartProblem { get; set; }
    public bool Asthma { get; set; }
    public bool FoodAllergies { get; set; }
    public bool Cancer { get; set; }
    public string OtherIllnesses { get; set; } = string.Empty;
    public string? MaintenanceMeds { get; set; } = string.Empty;
    public string? OBGyneHistory { get; set; } = string.Empty;
}
