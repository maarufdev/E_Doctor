namespace E_Doctor.Application.DTOs.Common.UserAccountDTOs;
public sealed record RegisterUserDTO
{
    public string? UserName { get; init; }
    public string? Password { get; init; }
    public string? FirstName { get; init; }
    public string? LastName { get; init; }
    public string? MiddleName { get; init; }
    public DateTime? DateOfBirth { get; init; }
}