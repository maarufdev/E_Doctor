namespace E_Doctor.Application.DTOs.ManageUsers.RequestDTOs;
public sealed record ResetPasswordRequest(
    int UserId,
    string Email,
    string Password
);
