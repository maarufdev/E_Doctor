namespace E_Doctor.Application.DTOs.ManageUsers.RequestDTOs;

public sealed record SaveManageUserRequest(
    int? UserId,
    string FirstName,
    string MiddleName,
    string LastName,
    DateTime? DateOfBirth,
    int UserStatusId,
    string Email,
    string Password
);

public static class ValidateManageUserRequest
{
    public static void Validate(this SaveManageUserRequest request)
    {
        ArgumentNullException.ThrowIfNull(request);

        ArgumentException.ThrowIfNullOrWhiteSpace(request.FirstName);
        ArgumentException.ThrowIfNullOrWhiteSpace(request.MiddleName);
        ArgumentException.ThrowIfNullOrWhiteSpace(request.LastName);
        ArgumentException.ThrowIfNullOrWhiteSpace(request.Email);
        
        if(request.UserId == 0 || request.UserId is null)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(request.Password);
        }

        if (request.DateOfBirth == null || request.DateOfBirth == DateTime.MinValue)
        {
            throw new ArgumentException("Date Of Birth is required.");
        }
    }
}