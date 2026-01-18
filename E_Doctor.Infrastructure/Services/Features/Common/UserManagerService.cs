using E_Doctor.Application.Constants;
using E_Doctor.Application.DTOs.Common;
using E_Doctor.Application.DTOs.Common.CustomResultDTOs;
using E_Doctor.Application.DTOs.Common.UserAccountDTOs;
using E_Doctor.Application.DTOs.ManageUsers.RequestDTOs;
using E_Doctor.Application.DTOs.ManageUsers.ResponseDTOs;
using E_Doctor.Application.Helpers;
using E_Doctor.Application.Interfaces.Features.Common;
using E_Doctor.Core.Constants.Enums;
using E_Doctor.Core.Domain.Entities.Patient;
using E_Doctor.Infrastructure.Constants;
using E_Doctor.Infrastructure.Data;
using E_Doctor.Infrastructure.Identity;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Timeouts;
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

    public async Task<Result> RegisterPatient(RegisterPatientDTO request)
    {
        try
        {
            if (request is null) return Result.Failure("Register Patient fields are empty.");
            
            request.Validate();

            // Check if user already exists
            var existingUser = await _userManager.FindByEmailAsync(request.UserName);

            if (existingUser != null)
            {
                return Result.Failure($"A user with username {request.UserName} is already exists.");
            }

            // Map DTO to AppUserIdentity

            var newUser = new AppUserIdentity
            {
                UserName = request.UserName,
                Email = request.UserName,
                FirstName = request.FirstName,
                LastName = request.LastName,
                MiddleName = request.MiddleName,
                DateOfBirth = request.DateOfBirth,
                EmailConfirmed = true,
                Status = (int)UserStatus.Active,
                IsActive = true,
                SecurityStamp = Guid.NewGuid().ToString("D")
            };

            // Create user
            var result = await _userManager.CreateAsync(newUser, request.Password);
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

            var userId = newUser.Id;

            var patientInfo = new PatientInformationEntity
            {
                UserId = userId,
                LastName = newUser.LastName,
                FirstName = newUser.FirstName,
                MiddleName = newUser.MiddleName,
                DateOfBirth = (DateTime)newUser.DateOfBirth,
                Religion = request.Religion,
                Gender = request.Gender,
                Address = request.Address,
                CivilStatus = request.CivilStatus,
                Nationality = request.Nationality,
                Occupation = request.Occupation,
                CreatedOn = DateTime.UtcNow,
                IsActive = true
            };

            var pastMedRecord = new PatientPastMedicalRecordEntity
            {
                PatientInfoId = patientInfo.Id
            };

            if (request.PatientPastMedicalRecord is not null)
            {
                var pastRec = request.PatientPastMedicalRecord;

                pastMedRecord.PreviousHospitalization = pastRec.PreviousHospitalization;
                pastMedRecord.PreviousHospitalizationText = pastRec.PreviousHospitalizationText;
                pastMedRecord.PastSurgery = pastRec.PastSurgery;
                pastMedRecord.Diabetes = pastRec.Diabetes;
                pastMedRecord.Hypertension = pastRec.Hypertension;
                pastMedRecord.AllergyToMeds = pastRec.AllergyToMeds;
                pastMedRecord.MedAllergyText = pastRec.MedAllergyText;
                pastMedRecord.HeartProblem = pastRec.HeartProblem;
                pastMedRecord.Asthma = pastRec.Asthma;
                pastMedRecord.FoodAllergies = pastRec.FoodAllergies;
                pastMedRecord.Cancer = pastRec.Cancer;
                pastMedRecord.OtherIllnesses = pastRec.OtherIllnesses ?? string.Empty;
                pastMedRecord.OBGyneHistory = pastRec.OBGyneHistory;
                pastMedRecord.MaintenanceMeds = pastRec.MaintenanceMeds;
            }
            
            patientInfo.PatientPastMedicalRecord = pastMedRecord;

            var personalHistory = new PatientPersonalHistoryEntity
            {
                PatientInfoId = patientInfo.Id
            };

            if (request.PatientPersonalHistory is not null)
            {
                var perHistDTO = request.PatientPersonalHistory;
                personalHistory.Smoker = perHistDTO.Smoker;
                personalHistory.AlchoholBeverageDrinker = perHistDTO.AlchoholBeverageDrinker;
                personalHistory.IllicitDrugUser = perHistDTO.IllicitDrugUser;
                personalHistory.Others = perHistDTO.Others;
            }

            patientInfo.PatientPersonalHistory = personalHistory;

            var famHistory = new PatientFamilyHistoryEntity
            {
                PatientInfoId = patientInfo.Id
            };

            if(request.PatientFamilyHistory is not null)
            {
                var famHistoryDTO = request.PatientFamilyHistory;
                famHistory.PTB = famHistoryDTO.PTB;
                famHistory.Hypertension = famHistoryDTO.Hypertension;
                famHistory.Cardiac = famHistoryDTO.Cardiac;
                famHistory.None = famHistoryDTO.None;
                famHistory.Diabetes = famHistoryDTO.Diabetes;
                famHistory.Asthma = famHistoryDTO.Asthma;
                famHistory.Cancer = famHistoryDTO.Cancer;
                famHistory.Others = famHistoryDTO.Others;
            }
            
            patientInfo.PatientFamilyHistory = famHistory;

            await _context.PatientInformations.AddAsync(patientInfo);
            var isSuccessSavePatientInfoResult = await _context.SaveChangesAsync() > 0;

            if (!isSuccessSavePatientInfoResult) return Result.Failure("Something went wrong saving patient info");

            await _activityLogger.LogAsync(
                newUser.Id,
                UserActivityTypes.Login,
                newUser.Email
            );

            return Result.Success();
        }
        catch(Exception ex)
        {
            Console.WriteLine(ex.Message.ToString());
            return Result.Failure("Something went wrong.");
        }
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
                userAccountToUpdate.LastLogInDate = DateTime.UtcNow;

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

    public async Task<Result<UserDetailsResponseDTO>> GetUserDetailsByUserId(int userId)
    {
        try
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());

            if (user is null) return Result<UserDetailsResponseDTO>.Failure("User not found.");

            var patientInfo = await _context.PatientInformations
                .Include(p => p.PatientPastMedicalRecord)
                .Include(p => p.PatientFamilyHistory)
                .Include(p => p.PatientPersonalHistory)
                .AsNoTracking()
                .Where(p => p.UserId == userId && p.IsActive)
                .FirstOrDefaultAsync();

            if (patientInfo is null)
            {
                var result = new UserDetailsResponseDTO
                {
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    MiddleName = user.MiddleName,
                    DateOfBirth = user.DateOfBirth
                };

                return Result<UserDetailsResponseDTO>.Success(result);
            }

            var patientDetailsResult = new UserDetailsResponseDTO
            {
                PatientInfoId = patientInfo.Id,
                FirstName = patientInfo.FirstName,
                LastName = patientInfo.LastName,
                MiddleName = patientInfo.MiddleName,
                DateOfBirth = patientInfo.DateOfBirth,
                Religion = patientInfo.Religion,
                Gender = patientInfo.Gender,
                Address = patientInfo.Address,
                CivilStatus = patientInfo.CivilStatus,
                Occupation = patientInfo.Occupation,
                Nationality = patientInfo.Nationality,
            };

            if (patientInfo.PatientPastMedicalRecord is not null)
            {
                var pastMedicalRecord = patientInfo.PatientPastMedicalRecord;

                patientDetailsResult = patientDetailsResult with
                {
                    PatientPastMedicalRecord = new PatientPastMedicalRecordDTO
                    {
                        PreviousHospitalization = pastMedicalRecord.PreviousHospitalization,
                        PreviousHospitalizationText = pastMedicalRecord.PreviousHospitalizationText,
                        PastSurgery = pastMedicalRecord.PastSurgery,
                        Diabetes = pastMedicalRecord.Diabetes,
                        Hypertension = pastMedicalRecord.Hypertension,
                        AllergyToMeds = pastMedicalRecord.AllergyToMeds,
                        MedAllergyText = pastMedicalRecord.MedAllergyText,
                        HeartProblem = pastMedicalRecord.HeartProblem,
                        Asthma = pastMedicalRecord.Asthma,
                        FoodAllergies = pastMedicalRecord.AllergyToMeds,
                        Cancer = pastMedicalRecord.Cancer,
                        OtherIllnesses = pastMedicalRecord.OtherIllnesses,
                        MaintenanceMeds = pastMedicalRecord.MaintenanceMeds,
                        OBGyneHistory = pastMedicalRecord.OBGyneHistory
                    }
                };
            }

            if (patientInfo.PatientFamilyHistory is not null)
            {
                var famHistory = patientInfo.PatientFamilyHistory;
                patientDetailsResult = patientDetailsResult with
                {
                    PatientFamilyHistory = new PatientFamilyHistoryDTO
                    {
                        PTB = famHistory.PTB,
                        Hypertension = famHistory.Hypertension,
                        Cardiac = famHistory.Cardiac,
                        None = famHistory.None,
                        Diabetes = famHistory.Diabetes,
                        Asthma = famHistory.Asthma,
                        Cancer = famHistory.Cancer,
                        Others = famHistory.Others,
                    }
                };
            }

            if (patientInfo.PatientPersonalHistory is not null)
            {
                var personalHistory = patientInfo.PatientPersonalHistory;
                patientDetailsResult = patientDetailsResult with
                {
                    PatientPersonalHistory = new PatientPersonalHistoryDTO
                    {
                        Smoker = personalHistory.Smoker,
                        AlchoholBeverageDrinker = personalHistory.AlchoholBeverageDrinker,
                        IllicitDrugUser = personalHistory.IllicitDrugUser,
                        Others = personalHistory.Others,
                    }
                };
            }

            return Result<UserDetailsResponseDTO>.Success(patientDetailsResult);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message.ToString());
            return Result<UserDetailsResponseDTO>.Failure("Something went wrong");
        }
    }

    public async Task<Result> InsertOrUpdateUserDetails(SaveUserDetailsRequest request)
    {
        try
        {
            var userId = await GetUserId();
            var userAccount = await _userManager.FindByIdAsync(userId.ToString());

            var patientInfo = await _context.PatientInformations
                .Include(p => p.PatientPastMedicalRecord)
                .Include(p => p.PatientFamilyHistory)
                .Include(p => p.PatientPersonalHistory)
                .Where(p => p.UserId == userId)
                .FirstOrDefaultAsync();

            if (patientInfo is not null)
            {
                patientInfo.UserId = userAccount.Id;
                patientInfo.LastName = request.LastName;
                patientInfo.FirstName = request.FirstName;
                patientInfo.MiddleName = request.MiddleName;
                patientInfo.DateOfBirth = request.DateOfBirth.Value;
                patientInfo.Religion = request.Religion;
                patientInfo.Gender = request.Gender;
                patientInfo.Address = request.Address;
                patientInfo.CivilStatus = request.CivilStatus;
                patientInfo.Occupation = request.Occupation;
                patientInfo.Nationality = request.Nationality;
                patientInfo.UpdatedOn = DateTime.UtcNow;

                if (request.PatientPastMedicalRecord is not null)
                {
                    var pastMedRec = request.PatientPastMedicalRecord;

                    patientInfo.PatientPastMedicalRecord.PreviousHospitalization = pastMedRec.PreviousHospitalization;
                    patientInfo.PatientPastMedicalRecord.PreviousHospitalizationText = pastMedRec.PreviousHospitalizationText;
                    patientInfo.PatientPastMedicalRecord.PastSurgery = pastMedRec.PastSurgery;
                    patientInfo.PatientPastMedicalRecord.Diabetes = pastMedRec.Diabetes;
                    patientInfo.PatientPastMedicalRecord.Hypertension = pastMedRec.Hypertension;
                    patientInfo.PatientPastMedicalRecord.AllergyToMeds = pastMedRec.AllergyToMeds;
                    patientInfo.PatientPastMedicalRecord.MedAllergyText = pastMedRec.MedAllergyText;
                    patientInfo.PatientPastMedicalRecord.HeartProblem = pastMedRec.HeartProblem;
                    patientInfo.PatientPastMedicalRecord.Asthma = pastMedRec.Asthma;
                    patientInfo.PatientPastMedicalRecord.FoodAllergies = pastMedRec.FoodAllergies;
                    patientInfo.PatientPastMedicalRecord.Cancer = pastMedRec.Cancer;
                    patientInfo.PatientPastMedicalRecord.OtherIllnesses = pastMedRec.OtherIllnesses ?? string.Empty;
                    patientInfo.PatientPastMedicalRecord.MaintenanceMeds = pastMedRec.MaintenanceMeds;
                    patientInfo.PatientPastMedicalRecord.OBGyneHistory = pastMedRec.OBGyneHistory;
                    patientInfo.PatientPastMedicalRecord.UpdatedOn = DateTime.UtcNow;
                }

                if (request.PatientFamilyHistory is not null)
                {
                    var famHistory = request.PatientFamilyHistory;

                    patientInfo.PatientFamilyHistory.PTB = famHistory.PTB;
                    patientInfo.PatientFamilyHistory.Hypertension = famHistory.Hypertension;
                    patientInfo.PatientFamilyHistory.Cardiac = famHistory.Cardiac;
                    patientInfo.PatientFamilyHistory.None = famHistory.None;
                    patientInfo.PatientFamilyHistory.Diabetes = famHistory.Diabetes;
                    patientInfo.PatientFamilyHistory.Asthma = famHistory.Asthma;
                    patientInfo.PatientFamilyHistory.Cancer = famHistory.Cancer;
                    patientInfo.PatientFamilyHistory.Others = famHistory.Others;
                    patientInfo.PatientFamilyHistory.UpdatedOn = DateTime.UtcNow;
                }

                if (request.PatientPersonalHistory is not null)
                {
                    var personalHistory = request.PatientPersonalHistory;

                    patientInfo.PatientPersonalHistory.Smoker = personalHistory.Smoker;
                    patientInfo.PatientPersonalHistory.AlchoholBeverageDrinker = personalHistory.AlchoholBeverageDrinker;
                    patientInfo.PatientPersonalHistory.IllicitDrugUser = personalHistory.IllicitDrugUser;
                    patientInfo.PatientPersonalHistory.Others = personalHistory.Others;
                    patientInfo.PatientPersonalHistory.UpdatedOn = DateTime.UtcNow;
                }
            }
            else
            {
                patientInfo = new PatientInformationEntity();
                patientInfo.UserId = userAccount.Id;
                patientInfo.LastName = request.LastName;
                patientInfo.FirstName = request.FirstName;
                patientInfo.MiddleName = request.MiddleName;
                patientInfo.DateOfBirth = request.DateOfBirth.Value;
                patientInfo.Religion = request.Religion;
                patientInfo.Gender = request.Gender;
                patientInfo.Address = request.Address;
                patientInfo.CivilStatus = request.CivilStatus;
                patientInfo.Occupation = request.Occupation;
                patientInfo.Nationality = request.Nationality;
                patientInfo.UpdatedOn = DateTime.UtcNow;
                patientInfo.IsActive = true;

                var pastMedRec = request.PatientPastMedicalRecord;
                patientInfo.PatientPastMedicalRecord = new();

                patientInfo.PatientPastMedicalRecord.PatientInfoId = patientInfo.Id;
                patientInfo.PatientPastMedicalRecord.PreviousHospitalization = pastMedRec.PreviousHospitalization;
                patientInfo.PatientPastMedicalRecord.PreviousHospitalizationText = pastMedRec.PreviousHospitalizationText;
                patientInfo.PatientPastMedicalRecord.PastSurgery = pastMedRec.PastSurgery;
                patientInfo.PatientPastMedicalRecord.Diabetes = pastMedRec.Diabetes;
                patientInfo.PatientPastMedicalRecord.Hypertension = pastMedRec.Hypertension;
                patientInfo.PatientPastMedicalRecord.AllergyToMeds = pastMedRec.AllergyToMeds;
                patientInfo.PatientPastMedicalRecord.MedAllergyText = pastMedRec.MedAllergyText;
                patientInfo.PatientPastMedicalRecord.HeartProblem = pastMedRec.HeartProblem;
                patientInfo.PatientPastMedicalRecord.Asthma = pastMedRec.Asthma;
                patientInfo.PatientPastMedicalRecord.FoodAllergies = pastMedRec.FoodAllergies;
                patientInfo.PatientPastMedicalRecord.Cancer = pastMedRec.Cancer;
                patientInfo.PatientPastMedicalRecord.OtherIllnesses = pastMedRec.OtherIllnesses ?? string.Empty;
                patientInfo.PatientPastMedicalRecord.MaintenanceMeds = pastMedRec.MaintenanceMeds;
                patientInfo.PatientPastMedicalRecord.OBGyneHistory = pastMedRec.OBGyneHistory;
                patientInfo.PatientPastMedicalRecord.CreatedOn = DateTime.UtcNow;
                patientInfo.PatientPastMedicalRecord.IsActive = true;

                var famHistory = request.PatientFamilyHistory;
                patientInfo.PatientFamilyHistory = new();

                patientInfo.PatientFamilyHistory.PatientInfoId = patientInfo.Id;
                patientInfo.PatientFamilyHistory.PTB = famHistory.PTB;
                patientInfo.PatientFamilyHistory.Hypertension = famHistory.Hypertension;
                patientInfo.PatientFamilyHistory.Cardiac = famHistory.Cardiac;
                patientInfo.PatientFamilyHistory.None = famHistory.None;
                patientInfo.PatientFamilyHistory.Diabetes = famHistory.Diabetes;
                patientInfo.PatientFamilyHistory.Asthma = famHistory.Asthma;
                patientInfo.PatientFamilyHistory.Cancer = famHistory.Cancer;
                patientInfo.PatientFamilyHistory.Others = famHistory.Others;
                patientInfo.PatientFamilyHistory.CreatedOn = DateTime.UtcNow;
                patientInfo.PatientFamilyHistory.IsActive = true;

                var personalHistory = request.PatientPersonalHistory;
                patientInfo.PatientPersonalHistory = new();

                patientInfo.PatientPersonalHistory.PatientInfoId = patientInfo.Id;
                patientInfo.PatientPersonalHistory.Smoker = personalHistory.Smoker;
                patientInfo.PatientPersonalHistory.AlchoholBeverageDrinker = personalHistory.AlchoholBeverageDrinker;
                patientInfo.PatientPersonalHistory.IllicitDrugUser = personalHistory.IllicitDrugUser;
                patientInfo.PatientPersonalHistory.Others = personalHistory.Others;
                patientInfo.PatientPersonalHistory.CreatedOn = DateTime.UtcNow;
                patientInfo.PatientPersonalHistory.IsActive = true;

                await _context.PatientInformations.AddAsync(patientInfo);
            }

            var isUpdateSuccess = await _context.SaveChangesAsync() > 0;

            if (!isUpdateSuccess) return Result.Failure("Something went wrong.");

            userAccount.FirstName = patientInfo.FirstName;
            userAccount.LastName = patientInfo.LastName;
            userAccount.MiddleName = patientInfo.MiddleName;
            userAccount.DateOfBirth = patientInfo.DateOfBirth;

            var accountUpdateResult = await _userManager.UpdateAsync(userAccount);

            if (!accountUpdateResult.Succeeded) return Result.Failure("Something went wrong during account updates");

            return Result.Success();
        }
        catch (Exception)
        {
            return Result.Failure("Something went wrong");
        }
    }

    public async Task<Result<UserDetailsResponseDTO>> GetUserDetailsByUserInfoId(int userInfoId)
    {
        try
        {
            var userInfoResponse = await _context.PatientInformations
            .AsNoTracking()
            .Where(p => p.Id == userInfoId)
            .Select(p => new { p.Id, p.UserId })
            .FirstOrDefaultAsync();

            if (userInfoResponse is null) return Result<UserDetailsResponseDTO>.Failure("User Info does not exists");

            var result = await GetUserDetailsByUserId(userInfoResponse.UserId);

            if (result.IsFailure) return Result<UserDetailsResponseDTO>.Failure(result.Error);

            return Result<UserDetailsResponseDTO>.Success(result.Value);
        }
        catch (Exception)
        {
            return Result<UserDetailsResponseDTO>.Failure("Something went wrong");
        }
    }
}