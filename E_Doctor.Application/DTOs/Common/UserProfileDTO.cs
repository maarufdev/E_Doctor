namespace E_Doctor.Application.DTOs.Common;
public sealed record UserProfileDTO(
    string DisplayInitial,
    string FullName,
    string Email,
    string DateOfBirth
    );
