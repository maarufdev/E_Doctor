using E_Doctor.Application.DTOs.Common;
using E_Doctor.Application.DTOs.Common.CustomResultDTOs;
using E_Doctor.Application.DTOs.Common.UserAccountDTOs;

namespace E_Doctor.Application.Interfaces.Features.Common;
public interface IUserManagerService
{
    Task<Result<string>> Login(LoginDTO loginDTO);
    Task<Result> Register(RegisterUserDTO registerDTO);
    Task<Result> Logout();
    Task<int?> GetUserId();
    Task<string> GetUserFullName();
    Task<string> GetUserFullNameByUserId();
    Task<UserProfileDTO> GetUserProfile();
    Task<Result> ResetPassword(ResetPasswordDTO resetPasswordDTO);
}