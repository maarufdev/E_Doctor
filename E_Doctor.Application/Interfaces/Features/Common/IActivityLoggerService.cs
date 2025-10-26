using E_Doctor.Core.Constants.Enums;
public interface IActivityLoggerService
{
    Task LogAsync(int? userId, UserActivityTypes type, string description);
}
