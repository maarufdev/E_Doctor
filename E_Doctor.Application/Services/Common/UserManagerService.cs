using E_Doctor.Application.DTOs.Common.CustomResultDTOs;
using E_Doctor.Application.DTOs.Common.UserAccountDTOs;
using E_Doctor.Application.Interfaces.Features.Common;
using E_Doctor.Infrastructure.Identity;
using E_Doctor.Infrastructure.Identity.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using System.Globalization;
using System.Security.Claims;

namespace E_Doctor.Application.Services.Common;

internal class UserManagerService : IUserManagerService
{
    private readonly UserManager<AppUserIdentity> _userManager;
    private readonly SignInManager<AppUserIdentity> _signInManager;
    public UserManagerService(
        UserManager<AppUserIdentity> userManager,
        SignInManager<AppUserIdentity> signInManager)
    {
        _userManager = userManager;
        _signInManager = signInManager;
    }
    public async Task<Result<string>> Login(LoginDTO loginDTO)
    {
        try
        {
            var user = await _userManager.FindByEmailAsync(loginDTO.Username);

            if (user is null)
                return Result<string>.Failure("User does not exist.");

            var isPasswordValid = await _userManager.CheckPasswordAsync(user, loginDTO.Password);
            if (!isPasswordValid)
                return Result<string>.Failure("Invalid Credentials.");

            var claims = new List<Claim>
            {
                new Claim("FirstName", user.FirstName ?? string.Empty),
                new Claim("FirstInitial", string.IsNullOrEmpty(user.FirstName) ? string.Empty : user.FirstName[0].ToString())
            };

            await _signInManager.SignInWithClaimsAsync(user, isPersistent: true, claims);

            return Result<string>.Success("Success");
        }
        catch (Exception)
        {
            return Result<string>.Failure("Something went wrong!");
        }
    }

    public async Task<Result> Logout()
    {
        await _signInManager.SignOutAsync();

        return Result.Success();
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


        return Result.Success();
    }
}