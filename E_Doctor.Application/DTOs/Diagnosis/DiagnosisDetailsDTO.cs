namespace E_Doctor.Application.DTOs.Diagnosis;
public sealed record DiagnosisDetailsDTO
{
    public string? Result { get; init; }
    public string? Description { get; init; }
    public string? Prescription { get; init; }

    public DiagnosisDetailsDTO()
    {
        
    }
    private DiagnosisDetailsDTO(string illnessResult, string description, string prescription)
    {
        Result = illnessResult;
        Description = description;
        Prescription = prescription;
    }

    public static DiagnosisDetailsDTO Create(string illnessResult, string description, string prescription)
    {
        return new DiagnosisDetailsDTO(illnessResult, description ,prescription);
    }
}