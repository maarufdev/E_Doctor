namespace E_Doctor.Application.Constants;
public enum UserStatus
{
    Deleted,
    Active,
    Archived,
}

public static class UserStatusHelper
{
    public static string GetUserStatusString(int userStatus)
    {
        return ((UserStatus)Enum.ToObject(typeof(UserStatus), userStatus)).ToString();
    }
}