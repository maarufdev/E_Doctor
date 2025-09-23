namespace E_Doctor.Application.DTOs.Diagnosis;
public sealed record DiagnosisDetailsDTO
{
    public string? Result { get; init; }
    public string? Description { get; init; }
    public string? Prescription { get; init; }
    public string? Notes { get; init; }

    public DiagnosisDetailsDTO()
    {
        
    }
    private DiagnosisDetailsDTO(string illnessResult, string description, string prescription, string notes)
    {
        Result = illnessResult;
        Description = description;
        Prescription = prescription;
        Notes = notes;
    }

    public static DiagnosisDetailsDTO Create(string illnessResult, string description, string prescription, string notes)
    {
        return new DiagnosisDetailsDTO(illnessResult, description ,prescription, notes);
    }
}