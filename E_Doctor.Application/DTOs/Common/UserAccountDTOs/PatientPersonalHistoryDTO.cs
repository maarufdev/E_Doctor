namespace E_Doctor.Application.DTOs.Common.UserAccountDTOs;
public sealed record PatientPersonalHistoryDTO
{
    public bool Smoker { get; init; }
    public bool AlchoholBeverageDrinker { get; init; }
    public bool IllicitDrugUser { get; set; }
    public string? Others { get; init; } = string.Empty;
}
