namespace E_Doctor.Core.Domain.Entities.Admin;
public class UserEntity
{
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? MiddleName { get; set; }
    public DateTime? DateOfBirth { get; set; }
    public List<string> Roles { get; set; } = new List<string>();
    public bool IsActive { get; set; }
    public DateTime UpdatedOn { get; set; } = DateTime.Now;
}