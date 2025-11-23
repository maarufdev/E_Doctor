namespace E_Doctor.Application.DTOs.Common.UserAccountDTOs;
public sealed record RegisterPatientDTO
{
    public string? UserName { get; init; }
    public string? Password { get; init; }
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

public static class RegisterPatientDTOHelper
{
    public static void Validate(this RegisterPatientDTO dto)
    {

        ArgumentException.ThrowIfNullOrEmpty(dto.UserName);
        ArgumentException.ThrowIfNullOrEmpty(dto.Password);
        ArgumentException.ThrowIfNullOrEmpty(dto.LastName);
        ArgumentException.ThrowIfNullOrEmpty(dto.FirstName);
        ArgumentException.ThrowIfNullOrEmpty(dto.MiddleName);
        ArgumentException.ThrowIfNullOrEmpty(dto.Gender);
        ArgumentException.ThrowIfNullOrEmpty(dto.Religion);
        ArgumentException.ThrowIfNullOrEmpty(dto.CivilStatus);
        ArgumentException.ThrowIfNullOrEmpty(dto.Nationality);

        if (dto.DateOfBirth is null) throw new ArgumentNullException(nameof(dto.DateOfBirth));

        if (dto.DateOfBirth.Value == DateTime.MinValue) throw new ArgumentException("Please provide a valid date of birth.");
    }
}