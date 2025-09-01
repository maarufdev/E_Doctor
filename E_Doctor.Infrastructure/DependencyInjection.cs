using E_Doctor.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace E_Doctor.Infrastructure;
public static class DependencyInjection
{
    public static IServiceCollection AddAdminInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<AdminAppDbContext>(options => options.UseSqlite(configuration.GetConnectionString("DefaultConnection")));

        return services;
    }

    public static IServiceCollection AddPatientInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<PatientAppDbContext>(options => options.UseSqlite(configuration.GetConnectionString("DefaultConnection")));

        return services;
    }
}
