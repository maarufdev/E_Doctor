namespace E_Doctor.Application.DTOs.Settings.RuleManagements;
public sealed record GetIllnessRequestDTO(
    string SearchText,
    int PageNumber,
    int PageSize
);