namespace E_Doctor.Core.Domain.Entities.Patient;

public class PatientInfoEntity : BaseEntity
{
    public int UserId { get; set; }
    public string? LastName { get; set; }
    public string? FirstName { get; set; }
    public DateTime DateOfBirth { get; set; }
    public string? MiddleName { get; set; }
    public string? Religion { get; set; }
    public string? Gender { get; set; }
    public string? Address { get; set; }
    public string? CivilStatus { get; set; }
    public string? Occupation { get; set; }
    public string? Nationality { get; set; }
    public int PatientPastMedicalRecordId  { get; set; }
    public PatientPastMedicalRecordEntity? PatientPastMedicalRecord  { get; set; }
    public int PatientFamilyHistoryId { get; set; }
    public PatientFamilyHistoryEntity? PatientFamilyHistory { get; set; }
    public int PatientPersonalHistoryId { get; set; }
    public PatientPersonalHistoryEntity? PatientPersonalHistory { get; set; }
}