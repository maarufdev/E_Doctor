namespace E_Doctor.Application.DTOs.ManageUsers.RequestDTOs;

public sealed record GetManageUserRequest(
    string SearchText,
    int? UserStatusId,
    int PageNumber,
    int PageSize
);