namespace E_Doctor.Application.DTOs.ManageUsers.ResponseDTOs;
public sealed record ManageUserResponse(
    int UserId,
    string FullName,
    string Email,
    int UserStatusId,
    string UserStatus,
    string LastLoggedIn
    );
