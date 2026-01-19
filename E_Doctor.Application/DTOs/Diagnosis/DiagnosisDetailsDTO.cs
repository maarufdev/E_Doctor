namespace E_Doctor.Application.DTOs.Diagnosis;
public sealed record DiagnosisDetailsDTO
{
    public IEnumerable<DiagnosisResultDTO> Result { get; init; } = [];
    public string? Notes { get; init; }
    public bool IsSuccess { get; set; } = true;

    public string[] Symptoms { get; init; } = Array.Empty<string>();

    private DiagnosisDetailsDTO()
    {
        
    }
    private DiagnosisDetailsDTO(
        IEnumerable<DiagnosisResultDTO> result, 
        string notes, 
        bool isSuccess = true)
    {
        Result = result;
        Notes = notes;
        Symptoms = Array.Empty<string>();
        IsSuccess = isSuccess;
    }

    private DiagnosisDetailsDTO(
        IEnumerable<DiagnosisResultDTO> result,
        string notes,
        string[] symptoms,
        bool isSuccess = true)
    {
        Result = result;
        Notes = notes;
        Symptoms = symptoms ?? Array.Empty<string>();
        IsSuccess = isSuccess;
    }

    public static DiagnosisDetailsDTO Create()
    {
        return new DiagnosisDetailsDTO();
    }
    //public static DiagnosisDetailsDTO Create(
    //    IEnumerable<DiagnosisResultDTO> result,
    //    string notes, 
    //    bool isSuccess = true)
    //{
    //    return new DiagnosisDetailsDTO(result, notes, isSuccess);
    //}

    public static DiagnosisDetailsDTO Create(
       IEnumerable<DiagnosisResultDTO> result,
        string notes, 
        string[] symptoms, 
        bool isSuccess = true)
    {
        return new DiagnosisDetailsDTO(result, notes, symptoms, isSuccess);
    }
}