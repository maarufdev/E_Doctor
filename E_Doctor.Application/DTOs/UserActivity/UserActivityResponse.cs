using E_Doctor.Core.Constants.Enums;

namespace E_Doctor.Application.DTOs.UserActivity;

public sealed record UserActivityResponse(
    string User,
    string DisplayDate,
    string ActivityType,
    string Description
    );
