using E_Doctor.Core.Constants.Enums;

namespace E_Doctor.Application.Helpers;
public static class ActivityDescriptionBuilder
{
    public static string Build(UserActivityTypes type, string? targetName = null)
    {
        return type switch
        {
            // General
            UserActivityTypes.Login => $"User {targetName} logged in.",
            UserActivityTypes.Register => $"New user {targetName} registered.",
            UserActivityTypes.Logout => $"User {targetName} logged out.",
            UserActivityTypes.ResetPassword => $"User {targetName} reset password.",

            // Illness Management
            UserActivityTypes.CreateIllness => $"Created illness '{targetName}'.",
            UserActivityTypes.UpdateIllness => $"Updated illness '{targetName}'.",
            UserActivityTypes.RemoveIllness => $"Removed illness '{targetName}'.",

            // Symptom Management
            UserActivityTypes.CreateSymptom => $"Created symptom '{targetName}'.",
            UserActivityTypes.UpdateSymptom => $"Updated symptom '{targetName}'.",
            UserActivityTypes.RemoveSymptom => $"Removed symptom '{targetName}'.",

            // Admin Actions
            UserActivityTypes.AdminCreateUser => $"Created new user '{targetName}'.",
            UserActivityTypes.AdminUpdateUser => $"Updated user '{targetName}'.",
            UserActivityTypes.AdminResetUserPassword => $"Reset password for user '{targetName}'.",
            UserActivityTypes.AdminReactivateUser => $"Reactivated user '{targetName}'.",
            UserActivityTypes.AdminArchiveUser => $"Archived user '{targetName}'.",
            UserActivityTypes.AdminDeleteDiagnosis => $"Admin deleted diagnosis record for user '{targetName}'.",
            UserActivityTypes.PatientDeleteDiagnosis => $"Deleted own diagnosis record.",
            // Patient Actions
            UserActivityTypes.PatientDiagnosis => $"Completed a diagnosis. Result: '{targetName ?? "Unknown"}'.",

            // System Tasks
            UserActivityTypes.SystemArchiveUser => $"User '{targetName}' was automatically archived by the system.",

            // Default fallback
            _ => $"Performed activity: {type}"
        };
    }
}