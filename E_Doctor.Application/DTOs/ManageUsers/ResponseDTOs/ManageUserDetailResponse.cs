namespace E_Doctor.Application.DTOs.ManageUsers.ResponseDTOs;
public sealed record ManageUserDetailResponse(
    int UserId,
    string FirstName,
    string MiddleName,
    string LastName,
    DateTime? DateOfBirth,
    int UserStatusId,
    string Email,
    string Password
);
