namespace E_Doctor.Application.DTOs.Diagnosis;
public sealed record DiagnosisDetailsDTO
{
    public string? Result { get; init; }
    public string? Description { get; init; }
    public string? Prescription { get; init; }
    public string? Notes { get; init; }
    public bool IsSuccess { get; set; } = true;

    public string[] Symptoms { get; init; } = Array.Empty<string>();

    private DiagnosisDetailsDTO()
    {
        
    }
    private DiagnosisDetailsDTO(
        string illnessResult, 
        string description, 
        string prescription, 
        string notes, 
        bool isSuccess = true)
    {
        Result = illnessResult;
        Description = description;
        Prescription = prescription;
        Notes = notes;
        Symptoms = Array.Empty<string>();
        IsSuccess = isSuccess;
    }

    private DiagnosisDetailsDTO(
        string illnessResult,
        string description,
        string prescription,
        string notes,
        string[] symptoms,
        bool isSuccess = true)
    {
        Result = illnessResult;
        Description = description;
        Prescription = prescription;
        Notes = notes;
        Symptoms = symptoms ?? Array.Empty<string>();
        IsSuccess = isSuccess;
    }

    public static DiagnosisDetailsDTO Create()
    {
        return new DiagnosisDetailsDTO();
    }
    public static DiagnosisDetailsDTO Create(
        string illnessResult, 
        string description, 
        string prescription, 
        string notes, 
        bool isSuccess = true)
    {
        return new DiagnosisDetailsDTO(illnessResult, description ,prescription, notes, isSuccess);
    }

    public static DiagnosisDetailsDTO Create(
        string illnessResult, 
        string description, 
        string prescription, 
        string notes, 
        string[] symptoms, 
        bool isSuccess = true)
    {
        return new DiagnosisDetailsDTO(illnessResult, description, prescription, notes, symptoms, isSuccess);
    }
}