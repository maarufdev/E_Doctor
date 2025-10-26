using E_Doctor.Core.Constants.Enums;

namespace E_Doctor.Core.Domain.Entities.Common;
public class UserActivityEntity
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public UserActivityTypes ActivityType { get; set; }
    public string? Description { get; set; } = string.Empty;
    public DateTime CreatedOn { get; set; } = DateTime.UtcNow;
}