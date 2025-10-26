using E_Doctor.Application.DTOs.Common;
using E_Doctor.Application.DTOs.Common.CustomResultDTOs;
using E_Doctor.Application.DTOs.UserActivity;
using E_Doctor.Application.Helpers;
using E_Doctor.Application.Interfaces.Features.Common;
using E_Doctor.Application.Interfaces.Features.UserActivity;
using E_Doctor.Infrastructure.Constants;
using E_Doctor.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace E_Doctor.Infrastructure.Services.Features.UserActivity;
internal class UserActivityService(
    AppDbContext context,
    IUserManagerService userManager
    ) : IUserActivityService
{
    private readonly IUserManagerService _userManager = userManager;
    private readonly AppDbContext _context = context;
    public async Task<Result<PagedResult<UserActivityResponse>>> GetAllUserActivities(GetPatientUserActivityRequest request)
    {
        try
        {
            var isAdmin = await _userManager.IsUserAdmin();

            var query =
                from activity in _context.UserActivities
                join user in _context.Users on activity.UserId equals user.Id
                join userRole in _context.UserRoles on user.Id equals userRole.UserId
                join role in _context.Roles on userRole.RoleId equals role.Id
                select new
                {
                    user.Email,
                    ActivityId = activity.Id,
                    UserId = user.Id,
                    FullName = $"{user.FirstName} {user.LastName}",
                    activity.ActivityType,
                    activity.Description,
                    activity.CreatedOn
                };

            if (!string.IsNullOrWhiteSpace(request.SearchText))
            {
                query = query.Where(q => q.Email.ToLower().Contains(request.SearchText.ToLower()));
            }

            query = query.OrderByDescending(x => x.CreatedOn);

            var totalCount = await query.CountAsync();

            var activities = (await query
                .Skip((request.PageNumber - 1) * request.PageSize)
                .Take(request.PageSize)
                .ToListAsync())
                .Select(x => new UserActivityResponse(
                    x.FullName,
                    DateTimeHelper.ToLocalShortDateTimeString(x.CreatedOn),
                    x.ActivityType.ToString(),
                    x.Description
                    ))
                .ToList();

            return await Task.FromResult(Result<PagedResult<UserActivityResponse>>.Success(new PagedResult<UserActivityResponse>
            {
                Items = activities,
                TotalCount = totalCount,
                PageNumber = request.PageNumber,
                PageSize = request.PageSize
            }));

        }
        catch ( Exception ex )
        {
            Console.WriteLine( ex.ToString() );
            return Result<PagedResult<UserActivityResponse>>.Failure("Something went wrong");
        }
    }

    public async Task<Result<PagedResult<UserActivityResponse>>> GetUserActivitiesById(GetPatientUserActivityRequest request)
    {
        try
        {
            var userId = await _userManager.GetUserId();
            var isAdmin = await _userManager.IsUserAdmin();

            var query =
                from activity in _context.UserActivities
                join user in _context.Users on activity.UserId equals user.Id
                join userRole in _context.UserRoles on user.Id equals userRole.UserId
                join role in _context.Roles on userRole.RoleId equals role.Id
                where role.Name == RoleConstants.Patient && user.Id == userId
                select new
                {
                    user.Email,
                    ActivityId = activity.Id,
                    UserId = user.Id,
                    FullName = $"{user.FirstName} {user.LastName}",
                    activity.ActivityType,
                    activity.Description,
                    activity.CreatedOn
                };

            query = query.OrderByDescending(x => x.CreatedOn);

            var totalCount = await query.CountAsync();

            var activities = (await query
                .Skip((request.PageNumber - 1) * request.PageSize)
                .Take(request.PageSize)
                .ToListAsync())
                .Select(x => new UserActivityResponse(
                    x.FullName,
                    DateTimeHelper.ToLocalShortDateTimeString(x.CreatedOn),
                    x.ActivityType.ToString(),
                    x.Description
                    ))
                .ToList();

            return await Task.FromResult(Result<PagedResult<UserActivityResponse>>.Success(new PagedResult<UserActivityResponse>
            {
                Items = activities,
                TotalCount = totalCount,
                PageNumber = request.PageNumber,
                PageSize = request.PageSize
            }));

        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.ToString());
            return Result<PagedResult<UserActivityResponse>>.Failure("Something went wrong");
        }
    }
}
