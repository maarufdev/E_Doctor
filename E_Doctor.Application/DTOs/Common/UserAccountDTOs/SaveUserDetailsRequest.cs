namespace E_Doctor.Application.DTOs.Common.UserAccountDTOs;
public sealed record SaveUserDetailsRequest
{
    public int PatientInfoId { get; set; }
    public string? FirstName { get; init; }
    public string? LastName { get; init; }
    public string? MiddleName { get; init; }
    public DateTime? DateOfBirth { get; init; }
    public string? Religion { get; init; }
    public string? Gender { get; init; }
    public string? Address { get; init; }
    public string? CivilStatus { get; init; }
    public string? Occupation { get; init; }
    public string? Nationality { get; init; }

    public PatientPastMedicalRecordDTO? PatientPastMedicalRecord { get; set; }
    public PatientFamilyHistoryDTO? PatientFamilyHistory { get; set; }
    public PatientPersonalHistoryDTO? PatientPersonalHistory { get; set; }
}
