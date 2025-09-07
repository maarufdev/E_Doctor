namespace E_Doctor.Application.DTOs.Common;
public sealed record LoginResultDTO(string? Token, bool Success, string? ErrorMessage);
