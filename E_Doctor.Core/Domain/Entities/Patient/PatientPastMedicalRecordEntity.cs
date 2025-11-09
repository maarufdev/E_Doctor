namespace E_Doctor.Core.Domain.Entities.Patient;
public class PatientPastMedicalRecordEntity : BaseEntity
{
    public int PatientInfoId { get; set; }
    public PatientInfoEntity? PatientInfoEntity { get; set; }
    public string? PreviousHospitalization { get; set; } = string.Empty;
    public string? PastSurgery { get; set; } = string.Empty;
    public string? Diabetes { get; set; } = string.Empty;
    public string? Hypertension { get; set; } = string.Empty;
    public string? AllergyToMeds { get; set; } = string.Empty;
    public string? HeartProblem { get; set; } = string.Empty;
    public string? Asthma { get; set; } = string.Empty;
    public string? FoodAllergies { get; set; } = string.Empty;
    public string? Cancer { get; set; } = string.Empty;
    public string? OtherIllnesses { get; set; } = string.Empty;
    public string? MaintenanceMeds { get; set; } = string.Empty;
    public string? OBGyneHistory { get; set; } = string.Empty;
}
