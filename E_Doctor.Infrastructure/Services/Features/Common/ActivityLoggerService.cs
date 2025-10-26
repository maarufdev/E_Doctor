using E_Doctor.Application.Helpers;
using E_Doctor.Core.Constants.Enums;
using E_Doctor.Core.Domain.Entities.Common;
using E_Doctor.Infrastructure.Data;

namespace E_Doctor.Infrastructure.Services.Features.Common
{
    internal class ActivityLoggerService : IActivityLoggerService
    {
        private readonly AppDbContext _context;
        public ActivityLoggerService(AppDbContext context)
        {
            _context = context;
        }
        public async Task LogAsync(
            int? userId,
            UserActivityTypes type, 
            string description)
        {
            try
            {
                var createdDescription = ActivityDescriptionBuilder.Build(type, description);

                var activity = new UserActivityEntity
                {
                    UserId = userId ?? 0,
                    ActivityType = type,
                    Description = createdDescription,
                };

                await _context.UserActivities.AddAsync(activity);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex) 
            { 
                Console.WriteLine(ex.ToString());
            }
        }
    }
}
