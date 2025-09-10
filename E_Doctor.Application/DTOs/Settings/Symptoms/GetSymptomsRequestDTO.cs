namespace E_Doctor.Application.DTOs.Settings.Symptoms;
public sealed record GetSymptomsRequestDTO(
    string SearchText,
    int PageNumber,
    int PageSize
);
