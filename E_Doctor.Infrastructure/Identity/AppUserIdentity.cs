using Microsoft.AspNetCore.Identity;

namespace E_Doctor.Infrastructure.Identity;

public class AppUserIdentity : IdentityUser<int>
{
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? MiddleName { get; set; }
    public DateTime? DateOfBirth { get; set; }
    public bool IsActive { get; set; }
    public int Status { get; set; }
    public DateTime UpdatedOn { get; set; } = DateTime.UtcNow;
}