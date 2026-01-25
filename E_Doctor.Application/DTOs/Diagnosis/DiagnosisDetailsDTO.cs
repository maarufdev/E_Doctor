namespace E_Doctor.Application.DTOs.Diagnosis;
public sealed record DiagnosisDetailsDTO
{
    public string? Result { get; init; }
    public string? DisclaimerTitle { get; init; }
    public string? DisclaimerDescription { get; init; }
    public bool IsSuccess { get; set; } = true;

    public string[] Symptoms { get; init; } = Array.Empty<string>();

    private DiagnosisDetailsDTO()
    {
        
    }
    private DiagnosisDetailsDTO(
        string illnessResult, 
        string discliamerTitle, 
        string disclaimerDesc, 
        bool isSuccess = true)
    {
        Result = illnessResult;
        DisclaimerTitle = discliamerTitle;
        DisclaimerDescription = disclaimerDesc;
        Symptoms = Array.Empty<string>();
        IsSuccess = isSuccess;
    }

    private DiagnosisDetailsDTO(
        string illnessResult,
         string discliamerTitle,
        string disclaimerDesc,
        string[] symptoms,
        bool isSuccess = true)
    {
        Result = illnessResult;
        DisclaimerTitle = discliamerTitle;
        DisclaimerDescription = disclaimerDesc;
        Symptoms = symptoms ?? Array.Empty<string>();
        IsSuccess = isSuccess;
    }

    public static DiagnosisDetailsDTO Create()
    {
        return new DiagnosisDetailsDTO();
    }
    public static DiagnosisDetailsDTO Create(
        string illnessResult, 
        string disclaimerTitle, 
        string disclaimerDesc, 
        string notes, 
        bool isSuccess = true)
    {
        return new DiagnosisDetailsDTO(illnessResult, disclaimerTitle ,disclaimerDesc, isSuccess);
    }

    public static DiagnosisDetailsDTO Create(
        string illnessResult, 
        string disclaimerTitle, 
        string disclaimerDesc, 
        string[] symptoms, 
        bool isSuccess = true)
    {
        return new DiagnosisDetailsDTO(illnessResult, disclaimerTitle, disclaimerDesc, symptoms, isSuccess);
    }
}