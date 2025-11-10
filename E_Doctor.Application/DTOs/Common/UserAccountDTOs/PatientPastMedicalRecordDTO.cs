namespace E_Doctor.Application.DTOs.Common.UserAccountDTOs;
public sealed record PatientPastMedicalRecordDTO
{
    public string? PreviousHospitalization { get; init; }
    public string? PastSurgery { get; init; }
    public string? Diabetes { get; init; }
    public string? Hypertension { get; init; }
    public string? AllergyToMeds { get; init; }
    public string? HeartProblem { get; init; }
    public string? Asthma { get; init; }
    public string? FoodAllergies { get; init; }
    public string? Cancer { get; init; }
    public string? OtherIllnesses { get; init; }
    public string? MaintenanceMeds { get; init; }
    public string? OBGyneHistory { get; init; }
}
