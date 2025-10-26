using E_Doctor.Core.Constants.Enums;
using E_Doctor.Infrastructure.Constants;
using E_Doctor.Infrastructure.Contants;
using E_Doctor.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace E_Doctor.Infrastructure.ApplicationBackgroundServices;

public class InActiveUserArchiverService : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly TimeSpan _checkInterval = TimeSpan.FromHours(1);

    public InActiveUserArchiverService(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            using var scope = _serviceProvider.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            var activityLoggerService = scope.ServiceProvider.GetRequiredService<IActivityLoggerService>();
            var thresholdDate = DateTime.UtcNow.AddDays(-30);
            
            var toArchiveUsers = await(
                from user in db.Users
                join userRole in db.UserRoles on user.Id equals userRole.UserId
                join role in db.Roles on userRole.RoleId equals role.Id
                where role.Name == RoleConstants.Patient
                    && (user.LastLogInDate != null ? user.LastLogInDate < thresholdDate : user.UpdatedOn < thresholdDate)
                    && user.Status != (int)UserStatus.Deleted
                    && user.Status != (int)UserStatus.Archived
                    && user.IsActive == true
                select user
            ).ToListAsync(stoppingToken);

            toArchiveUsers.ForEach(item => item.Status = 2);

            if (toArchiveUsers.Any())
            {
                await db.SaveChangesAsync(stoppingToken);

                foreach(var user  in toArchiveUsers)
                {
                    await activityLoggerService.LogAsync(
                        user.Id,
                        UserActivityTypes.SystemArchiveUser,
                        user.Email ?? string.Empty
                        );
                }
            }

            await Task.Delay(_checkInterval, stoppingToken);
        }
    }
}