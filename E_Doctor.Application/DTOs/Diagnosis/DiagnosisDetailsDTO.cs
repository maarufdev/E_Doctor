namespace E_Doctor.Application.DTOs.Diagnosis;
public sealed record DiagnosisDetailsDTO
{
    public string? Result { get; init; }
    public string? Description { get; init; }
    public string? Prescription { get; init; }
    public string? Notes { get; init; }
    public bool IsSuccess { get; set; } = true;

    public DiagnosisDetailsDTO()
    {
        
    }
    private DiagnosisDetailsDTO(string illnessResult, string description, string prescription, string notes, bool isSuccess = true)
    {
        Result = illnessResult;
        Description = description;
        Prescription = prescription;
        Notes = notes;
        IsSuccess = isSuccess;
    }

    public static DiagnosisDetailsDTO Create(string illnessResult, string description, string prescription, string notes, bool isSuccess = true)
    {
        return new DiagnosisDetailsDTO(illnessResult, description ,prescription, notes, isSuccess);
    }
}