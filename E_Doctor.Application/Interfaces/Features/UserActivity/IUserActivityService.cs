using E_Doctor.Application.DTOs.Common;
using E_Doctor.Application.DTOs.Common.CustomResultDTOs;
using E_Doctor.Application.DTOs.UserActivity;

namespace E_Doctor.Application.Interfaces.Features.UserActivity;
public interface IUserActivityService
{
    Task<Result<PagedResult<UserActivityResponse>>> GetAllUserActivities(GetPatientUserActivityRequest request);
    Task<Result<PagedResult<UserActivityResponse>>> GetUserActivitiesById(GetPatientUserActivityRequest request);
}