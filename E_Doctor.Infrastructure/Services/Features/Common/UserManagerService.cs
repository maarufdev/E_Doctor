using E_Doctor.Application.Constants;
using E_Doctor.Application.DTOs.Common;
using E_Doctor.Application.DTOs.Common.CustomResultDTOs;
using E_Doctor.Application.DTOs.Common.UserAccountDTOs;
using E_Doctor.Application.DTOs.ManageUsers.RequestDTOs;
using E_Doctor.Application.DTOs.ManageUsers.ResponseDTOs;
using E_Doctor.Application.Helpers;
using E_Doctor.Application.Interfaces.Features.Common;
using E_Doctor.Core.Constants.Enums;
using E_Doctor.Infrastructure.Constants;
using E_Doctor.Infrastructure.Data;
using E_Doctor.Infrastructure.Identity;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace E_Doctor.Infrastructure.Services.Features.Common;

internal class UserManagerService : IUserManagerService
{
    private readonly AppDbContext _context;
    private readonly UserManager<AppUserIdentity> _userManager;
    private readonly SignInManager<AppUserIdentity> _signInManager;
    private readonly RoleManager<IdentityRole<int>> _roleManager;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IActivityLoggerService _activityLogger;
    public UserManagerService(
        AppDbContext context,
        UserManager<AppUserIdentity> userManager,
        SignInManager<AppUserIdentity> signInManager,
        IHttpContextAccessor contextAccessor,
        RoleManager<IdentityRole<int>> roleManeger,
        IActivityLoggerService activityLogger
        )
    {
        _context = context;
        _userManager = userManager;
        _signInManager = signInManager;
        _httpContextAccessor = contextAccessor;
        _roleManager = roleManeger;
        _activityLogger = activityLogger;
    }

    public async Task<Result> DeleteUserById(int userId)
    {
        try
        {
            var user = await _userManager.Users
                .Where(u => u.Id == userId)
                .FirstOrDefaultAsync();

            if (user is null) return Result.Failure("User cannot be found.");

            user.IsActive = false;

            var result = await _userManager.UpdateAsync(user);

            if (!result.Succeeded) return Result.Failure($"Failed to delete user with id {user.UserName}");

            var adminUserId = await GetUserId();

            await _activityLogger.LogAsync(
                adminUserId,
                UserActivityTypes.AdminDeletedUser,
                user.UserName
                );

            await _activityLogger.LogAsync(
                userId,
                UserActivityTypes.AdminDeletedUser,
                user.UserName
                );

            return Result.Success();
        }
        catch(Exception)
        {
            return Result.Failure("Exception was thrown");
        }
    }

    public async Task<Result<ManageUserDetailResponse>> GetManageUserById(int userId)
    {
        var user = await _userManager.Users
            .AsNoTracking()
            .Where(u => u.Id == userId)
            .FirstOrDefaultAsync();

        if (user == null) return Result<ManageUserDetailResponse>.Failure("User Cannot Be found");

        var displayStatus = UserStatusHelper.GetUserStatusString(user.Status);
        var result = new ManageUserDetailResponse(
            user.Id,
            user.FirstName,
            user.MiddleName,
            user.LastName,
            user.DateOfBirth,
            user.Status,
            user.Email,
            string.Empty
            );

        return Result<ManageUserDetailResponse>.Success(result);
    }

    public async Task<Result<PagedResult<ManageUserResponse>>> GetManageUsers(GetManageUserRequest getManageUserRequest)
    {
        var query = _userManager.Users
            .AsNoTracking()
            .Where(u => u.IsActive)
            .OrderBy(u => u.LastName)
            .AsQueryable();

        query = query
            .Where(u => _context.UserRoles
            .Any(ur => ur.UserId == u.Id && _context.Roles
                .Any(r => r.Id == ur.RoleId && r.Name == RoleConstants.Patient))
            );

        if (!string.IsNullOrWhiteSpace(getManageUserRequest.SearchText))
        {
            var searchText = getManageUserRequest.SearchText.ToLower();

            query = query
                .Where(u =>
                    u.FirstName.ToLower().Contains(searchText) ||
                    u.LastName.ToLower().Contains(searchText) ||
                    u.MiddleName.ToLower().Contains(searchText) ||
                    u.Email.ToLower().Contains(searchText)
                    );
        }

        if(getManageUserRequest.UserStatusId is not null)
        {
            query = query.Where(u => u.Status == getManageUserRequest.UserStatusId);
        }

        var totalCounts = await query.CountAsync();

        var users = (
            await query
            .Skip((getManageUserRequest.PageNumber - 1) * getManageUserRequest.PageSize)
            .Take(getManageUserRequest.PageSize)
            .ToListAsync()
            )
            .Select(u => new ManageUserResponse(
                u.Id,
                $"{u.FirstName} {u.LastName}",
                u.Email,
                (int)u.Status,
                UserStatusHelper.GetUserStatusString(u.Status),
                u.LastLogInDate != null ?  DateTimeHelper.ToLocalShortDateTimeString(u.LastLogInDate.Value) : string.Empty 
                ))
            .ToList();
        
        var result = new PagedResult<ManageUserResponse>
        {
            Items = users,
            TotalCount = totalCounts,
            PageNumber = getManageUserRequest.PageNumber,
            PageSize = getManageUserRequest.PageSize,
        };

        return Result<PagedResult<ManageUserResponse>>.Success(result);
    }

    public async Task<string> GetUserFullName()
    {
        var userId = await GetUserId();
        var user = await _userManager.FindByIdAsync(userId.ToString());
        var fullName = $"{user?.FirstName} {user?.LastName}";

        return fullName ?? string.Empty;
    }

    public Task<string> GetUserFullNameByUserId()
    {
        throw new NotImplementedException();
    }

    public Task<int?> GetUserId()
    {
        var user = _httpContextAccessor.HttpContext?.User;

        int? userId = null;

        if (user == null || !user.Identity!.IsAuthenticated)
            return Task.FromResult(userId);

        var userIdClaim = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if(int.TryParse(userIdClaim, out var parsedUserId))
        {
           userId = parsedUserId;
        }

        return Task.FromResult(userId);
    }

    public async Task<bool> IsUserAdmin()
    {
        var userId = await GetUserId();

        var user = await _userManager.FindByIdAsync(userId?.ToString());
        if (user == null) return false;

        var isAdmin = await _userManager.IsInRoleAsync(user, RoleConstants.Admin);

        return isAdmin;
    }

    public async Task<UserProfileDTO> GetUserProfile()
    {
        var userId = await GetUserId();
        var user = await _userManager.FindByIdAsync(userId.ToString());

        var dob = user?.DateOfBirth != null ? user.DateOfBirth?.ToString("MMMM dd, yyyy") : string.Empty;
        string nameInitial = user?.FirstName[0].ToString() + user?.LastName[0].ToString();
        var fullName = await GetUserFullName();

        var result = new UserProfileDTO(
                nameInitial,
                fullName,
                user?.Email ?? string.Empty,
                dob ?? string.Empty
            );
        
        return result;
    }

    public async Task<Result<string>> Login(LoginDTO loginDTO)
    {
        try
        {
            // var user = await _userManager.FindByEmailAsync(loginDTO.Username);

            // if (user is null)
            //     return Result<string>.Failure("User does not exist.");

            // var isUserAdmin = await IsUserAdmin();
            // if (!isUserAdmin)
            // {
            //     var isArchived = user.Status == (int)UserStatus.Archived;
            //     if (isArchived) return Result<string>.Failure("You're account is not active. Please go to system admin to re-activate your account.");
            // }

            // var isPasswordValid = await _userManager.CheckPasswordAsync(user, loginDTO.Password);
            // if (!isPasswordValid)
            //     return Result<string>.Failure("Invalid Credentials.");

            // var claims = new List<Claim>
            // {
            //     new Claim("FirstName", user.FirstName ?? string.Empty),
            //     new Claim("FirstInitial", string.IsNullOrEmpty(user.FirstName) ? string.Empty : user.FirstName[0].ToString())
            // };

            //await _signInManager.SignInWithClaimsAsync(user, isPersistent: true, claims);

            // return Result<string>.Success("Success");

            ArgumentException.ThrowIfNullOrWhiteSpace(loginDTO.Username);
            ArgumentException.ThrowIfNullOrWhiteSpace(loginDTO.Password);

            var user = await _userManager.FindByEmailAsync(loginDTO.Username);

            if (user is null)
                return Result<string>.Failure("User does not exist.");

            if (user.Status == (int)UserStatus.Archived)
                return Result<string>.Failure("Account is inactive.");

            // Let ASP.NET handle password validation & cookie creation
            var result = await _signInManager.PasswordSignInAsync(user.UserName, loginDTO.Password, true, lockoutOnFailure: true);

            if (!result.Succeeded)
                return Result<string>.Failure("Invalid credentials.");

            user.LastLogInDate = DateTime.UtcNow;
            
            var updateResult = await _userManager.UpdateAsync(user);

            if (!updateResult.Succeeded)
            {
                return Result<string>.Failure("Something went wrong.");
            }

            // Now add extra claims if needed
            await _signInManager.SignOutAsync(); // Remove old cookie

            var claims = new List<Claim>
            {
                new Claim("FirstName", user.FirstName ?? string.Empty),
                new Claim("FirstInitial", string.IsNullOrEmpty(user.FirstName) ? string.Empty : user.FirstName[0].ToString())
            };

            await _signInManager.SignInWithClaimsAsync(user, isPersistent: true, claims);

            await _activityLogger.LogAsync(
                    user.Id,
                    UserActivityTypes.Login,
                    user.UserName
                    );

            return Result<string>.Success("Success");

        }
        catch (Exception)
        {
            return Result<string>.Failure("Something went wrong!");
        }
    }

    public async Task<Result> Logout()
    {
        var userId = await GetUserId();
        var currentUser = await GetCurrentUser(userId.ToString());

        await _signInManager.SignOutAsync();
        await _activityLogger.LogAsync(
                    userId,
                    UserActivityTypes.Logout,
                    currentUser.Email ?? string.Empty
                    );

        return Result.Success();
    }

    public async Task UpdateUserLoginDate()
    {
        var userId = await GetUserId();
        var userAccountToUpdate = await _userManager.FindByIdAsync(userId.ToString()!);

        if(userAccountToUpdate != null)
        {
            userAccountToUpdate.LastLogInDate = DateTime.UtcNow;
            await _userManager.UpdateAsync(userAccountToUpdate);
        }
    }

    private async Task<AppUserIdentity> GetCurrentUser(string userId)
    {
        return await _userManager.FindByIdAsync(userId);
    }

    public async Task<string> GetUserNameById(string userId)
    {
        var user = await _userManager.FindByIdAsync(userId);

        return user?.Email ?? string.Empty;
    }

    public async Task<Result> Register(RegisterUserDTO registerDTO)
    {
        // Validate incoming DTO
        if (string.IsNullOrWhiteSpace(registerDTO.UserName) ||
            string.IsNullOrWhiteSpace(registerDTO.Password) ||
            string.IsNullOrWhiteSpace(registerDTO.FirstName) ||
            string.IsNullOrWhiteSpace(registerDTO.LastName))
        {
            return Result.Failure("Required fields are missing.");
        }

        // Check if user already exists
        var existingUser = await _userManager.FindByEmailAsync(registerDTO.UserName);
        
        if (existingUser != null)
        {
            return Result.Failure("A user with that email/username already exists.");
        }

        // Map DTO to AppUserIdentity

        var newUser = new AppUserIdentity
        {
            UserName = registerDTO.UserName,
            Email = registerDTO.UserName,
            FirstName = registerDTO.FirstName,
            LastName = registerDTO.LastName,
            MiddleName = registerDTO.MiddleName,
            DateOfBirth = registerDTO.DateOfBirth,
            EmailConfirmed = true,      
            Status = (int)UserStatus.Active,
            IsActive = true,                    
            SecurityStamp = Guid.NewGuid().ToString("D")
        };

        // Create user
        var result = await _userManager.CreateAsync(newUser, registerDTO.Password);
        if (!result.Succeeded)
        {
            // Collect all error messages into one readable string
            var errors = string.Join("; ", result.Errors.Select(e => e.Description));
            return Result.Failure(errors);
        }

        result = await _userManager.AddToRoleAsync(newUser, RoleConstants.Patient);

        if (!result.Succeeded)
        {
            return Result.Failure("Something Went Wrong.");
        }

        await _activityLogger.LogAsync(
            newUser.Id,
            UserActivityTypes.Login,
            newUser.Email
        );

        return Result.Success();
    }

    public async Task<Result> ResetPassword(ResetPasswordDTO resetPasswordDTO)
    {
        var user = await _userManager.FindByEmailAsync(resetPasswordDTO.Username);
        
        if (user is null) return Result.Failure("User not found");

        var generatedToken = await _userManager.GeneratePasswordResetTokenAsync(user);

        var result = await _userManager.ResetPasswordAsync(user, generatedToken, resetPasswordDTO.NewPassword);

        if (!result.Succeeded)
        {
            return Result.Failure("Password was not reset successfully.");
        }

        await _activityLogger.LogAsync(
              user.Id,
              UserActivityTypes.ResetPassword,
              user.Email ?? string.Empty
          );

        return Result.Success();
    }

    public async Task<Result> ResetUserPasswordByAdmin(ResetPasswordRequest resetPasswordDTO)
    {
        try
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(resetPasswordDTO.Password);

            var user = await _userManager.FindByEmailAsync(resetPasswordDTO.Email);

            if (user is null) return Result.Failure("User does not exist.");

            var generatedTokenForReset = await _userManager.GeneratePasswordResetTokenAsync(user);
            var result = await _userManager.ResetPasswordAsync(user, generatedTokenForReset, resetPasswordDTO.Password);

            if (!result.Succeeded) return Result.Failure("Something went wrong during password reset.");

            var adminUserId = await GetUserId();
            
            await _activityLogger.LogAsync(
                adminUserId,
                UserActivityTypes.AdminResetUserPassword,
                user.Email ?? string.Empty
                );

            await _activityLogger.LogAsync(
                user.Id,
                UserActivityTypes.AdminResetUserPassword,
                user.Email ?? string.Empty
                );

            return Result.Success();
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.ToString());
            return Result.Failure("Something went wrong during password reset.");
        }
    }

    public async Task<Result<string>> SaveManagePatientAccount(SaveManageUserRequest saveManageUserRequest)
    {
        try
        {
            ArgumentNullException.ThrowIfNull(saveManageUserRequest);
            saveManageUserRequest.Validate();

            IdentityResult identityResult;
            string activityDescription = string.Empty;
            UserActivityTypes activityType;

            // Create
            if (saveManageUserRequest.UserId == 0 || saveManageUserRequest.UserId is null)
            {
                var userEmail = await _userManager.FindByEmailAsync(saveManageUserRequest.Email);

                if (userEmail is not null)
                {
                    return Result<string>.Failure("Email is already used");
                }

                var userToCreate = new AppUserIdentity
                {
                    UserName = saveManageUserRequest.Email,
                    Email = saveManageUserRequest.Email,
                    FirstName = saveManageUserRequest.FirstName,
                    MiddleName = saveManageUserRequest.MiddleName,
                    LastName = saveManageUserRequest.LastName,
                    DateOfBirth = saveManageUserRequest.DateOfBirth,
                    Status = saveManageUserRequest.UserStatusId,
                    EmailConfirmed = true,
                    IsActive = true,
                    SecurityStamp = Guid.NewGuid().ToString("D"),
                };

                identityResult = await _userManager.CreateAsync(userToCreate, saveManageUserRequest.Password);

                if (!identityResult.Succeeded)
                {
                    return Result<string>.Failure("Something Went Wrong.");
                }

                identityResult = await _userManager.AddToRoleAsync(userToCreate, RoleConstants.Patient);

                if (!identityResult.Succeeded)
                {
                    return Result<string>.Failure("Something Went Wrong.");
                }

                activityDescription = userToCreate.Email;
                activityType = UserActivityTypes.AdminCreateUser;

            }
            // Update
            else
            {
                var userAccountToUpdate = await _userManager.FindByIdAsync(saveManageUserRequest.UserId.ToString()!);

                if (userAccountToUpdate is null) return Result<string>.Failure("User doesn't exist!");

                if (userAccountToUpdate.Email != saveManageUserRequest.Email)
                {
                    identityResult = await _userManager.SetEmailAsync(userAccountToUpdate, saveManageUserRequest.Email);

                    if (!identityResult.Succeeded) return Result<string>.Failure("Something Went Wrong!");
                }

                userAccountToUpdate.Email = saveManageUserRequest.Email;
                userAccountToUpdate.UserName = saveManageUserRequest.Email;
                userAccountToUpdate.FirstName = saveManageUserRequest.FirstName;
                userAccountToUpdate.MiddleName = saveManageUserRequest.MiddleName;
                userAccountToUpdate.LastName = saveManageUserRequest.LastName;
                userAccountToUpdate.Status = saveManageUserRequest.UserStatusId;
                userAccountToUpdate.DateOfBirth = saveManageUserRequest.DateOfBirth;

                identityResult = await _userManager.UpdateAsync(userAccountToUpdate);

                if (!identityResult.Succeeded) return Result<string>.Failure("Something Went Wrong!");

                activityDescription = userAccountToUpdate.Email;
                activityType = UserActivityTypes.AdminUpdateUser;
            }

            await _activityLogger.LogAsync(
                await GetUserId(),
                activityType,
                activityDescription
                );


            return Result<string>.Success("Success");
        }
        catch (Exception)
        {
            return Result<string>.Failure("Something Went Wrong!");
        }
    }
}