namespace E_Doctor.Application.DTOs.UserActivity;
public sealed record GetPatientUserActivityRequest(
    string SearchText,
    int PageNumber,
    int PageSize
);