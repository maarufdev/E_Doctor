namespace E_Doctor.Application.DTOs.Common.UserAccountDTOs;
public sealed record PatientPastMedicalRecordDTO
{
    public bool PreviousHospitalization { get; init; }
    public string PreviousHospitalizationText { get; set; } = string.Empty;
    public bool PastSurgery { get; init; }
    public bool Diabetes { get; init; }
    public bool Hypertension { get; init; }
    public bool AllergyToMeds { get; init; }
    public string MedAllergyText { get; set; } = string.Empty;
    public bool HeartProblem { get; init; }
    public bool Asthma { get; init; }
    public bool FoodAllergies { get; init; }
    public bool Cancer { get; init; }
    public string? OtherIllnesses { get; init; } = string.Empty;
    public string? MaintenanceMeds { get; init; } = string.Empty;
    public string? OBGyneHistory { get; init; } = string.Empty;
}
