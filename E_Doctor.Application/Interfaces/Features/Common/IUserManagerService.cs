using E_Doctor.Application.DTOs.Common;
using E_Doctor.Application.DTOs.Common.CustomResultDTOs;
using E_Doctor.Application.DTOs.Common.UserAccountDTOs;
using E_Doctor.Application.DTOs.ManageUsers.RequestDTOs;
using E_Doctor.Application.DTOs.ManageUsers.ResponseDTOs;

namespace E_Doctor.Application.Interfaces.Features.Common;
public interface IUserManagerService
{
    Task<Result<string>> Login(LoginDTO loginDTO);
    Task<Result> Register(RegisterUserDTO registerDTO);
    Task UpdateUserLoginDate();
    Task<Result> Logout();
    Task<string> GetUserNameById(string userId);
    Task<int?> GetUserId();
    Task<string> GetUserFullName();
    Task<string> GetUserFullNameByUserId();
    Task<UserProfileDTO> GetUserProfile();
    Task<Result> ResetPassword(ResetPasswordDTO resetPasswordDTO);
    Task<Result<PagedResult<ManageUserResponse>>> GetManageUsers(GetManageUserRequest getManageUserRequest);
    Task<Result<string>> SaveManagePatientAccount(SaveManageUserRequest saveManageUserRequest);
    Task<Result<ManageUserDetailResponse>> GetManageUserById(int userId);
    Task<Result> DeleteUserById(int userId);
    Task<Result> ResetUserPasswordByAdmin(ResetPasswordRequest resetPasswordDTO);
    Task<bool> IsUserAdmin();
}