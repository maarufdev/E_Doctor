namespace E_Doctor.Application.DTOs.Common.UserAccountDTOs;
public sealed record PatientFamilyHistoryDTO
{
    public bool PTB { get; init; }
    public bool Hypertension { get; init; }
    public bool Cardiac { get; init; }
    public bool None { get; init; }
    public bool Diabetes { get; init; }
    public bool Asthma { get; init; }
    public bool Cancer { get; init; }
    public string? Others { get; init; }
}
