using E_Doctor.Infrastructure.Data;
using E_Doctor.Infrastructure.Identity;
using E_Doctor.Infrastructure.Identity.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.ComponentModel.DataAnnotations;

namespace E_Doctor.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddAdminInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services
            .AddDbContext<AdminAppDbContext>(options => options.UseSqlite(configuration.GetConnectionString("DefaultConnection")))
            .AddAdminCustomIdentityServices();

        return services;
    }

    public static IServiceCollection AddPatientInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services
            .AddDbContext<PatientAppDbContext>(options => options.UseSqlite(configuration.GetConnectionString("DefaultConnection")))
            .AddPatientCustomIdentityServices();
        
        return services;
    }

    private static IServiceCollection AddAdminCustomIdentityServices(this IServiceCollection services)
    {

        // We then add Identity services, which will automatically use the cookie configuration above.
        services.AddIdentity<AppUserIdentity, IdentityRole<int>>(options =>
        {
            options.Password.RequireUppercase = false;            // no uppercase required
            options.Password.RequireDigit = false;                // optional: no digit required
            options.Password.RequireNonAlphanumeric = false;      // optional: no special chars required
            options.Password.RequireLowercase = false;            // optional: no lowercase required
            options.Password.RequiredLength = 6;                  // optional: set min length
            options.Password.RequiredUniqueChars = 1;
        })
            .AddEntityFrameworkStores<AdminAppDbContext>()
            .AddDefaultTokenProviders();

        // We configure the cookie authentication settings first.
        services.ConfigureApplicationCookie(options =>
        {
            options.LoginPath = "/Account/Login";
            options.AccessDeniedPath = "/Account/Login";
        });

        services.AddSession(o =>
        {
            o.IdleTimeout = TimeSpan.FromMinutes(100);
            o.Cookie.HttpOnly = true;
            o.Cookie.IsEssential = true;
        });

        return services;
    }

    private static IServiceCollection AddPatientCustomIdentityServices(this IServiceCollection services)
    {

        // We then add Identity services, which will automatically use the cookie configuration above.
        services.AddIdentity<AppUserIdentity, IdentityRole<int>>(options =>
        {
            options.Password.RequireUppercase = false;            // no uppercase required
            options.Password.RequireDigit = false;                // optional: no digit required
            options.Password.RequireNonAlphanumeric = false;      // optional: no special chars required
            options.Password.RequireLowercase = false;            // optional: no lowercase required
            options.Password.RequiredLength = 6;                  // optional: set min length
            options.Password.RequiredUniqueChars = 1;
        })
            .AddEntityFrameworkStores<PatientAppDbContext>()
            .AddDefaultTokenProviders();

        // We configure the cookie authentication settings first.
        services.ConfigureApplicationCookie(options =>
        {
            options.LoginPath = "/Account/Login";
            options.AccessDeniedPath = "/Account/Login";
        });

        services.AddSession(o =>
        {
            o.IdleTimeout = TimeSpan.FromMinutes(100);
            o.Cookie.HttpOnly = true;
            o.Cookie.IsEssential = true;
        });

        return services;
    }

    public static async Task<IApplicationBuilder> SeedAdminUser([Required] this IApplicationBuilder app, IServiceProvider services, IConfiguration config)
    {
        ArgumentNullException.ThrowIfNull(services, nameof(services));

        using var scope = services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<AdminAppDbContext>();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<AppUserIdentity>>();

        ArgumentNullException.ThrowIfNull(context, nameof(context));

        var username = config["InitialUser:Username"];
        var password = config["InitialUser:Password"];
        var firstName = config["InitialUser:FirstName"];
        var lastName = config["InitialUser:LastName"];

        var isValidCreds = (!string.IsNullOrWhiteSpace(username) && !string.IsNullOrWhiteSpace(password));

        if (!isValidCreds) return app;

        var userToCreate = new AppUserIdentity
        {
            Email = username,
            NormalizedEmail = username!.ToUpper(),
            UserName = username,
            NormalizedUserName = username.ToUpper(),
            FirstName = firstName,
            LastName = lastName,
            DateOfBirth = DateTime.UtcNow,
            PhoneNumber = "+111111111111",
            EmailConfirmed = true,
            PhoneNumberConfirmed = true,
            IsActive = true,
            SecurityStamp = Guid.NewGuid().ToString("D")
        };

        var user = await userManager.FindByNameAsync(username);

        if (user is not null) return app;

        var result = await userManager.CreateAsync(userToCreate, password);

        if (!result.Succeeded)
        {
            throw new Exception("Failed to create initial user: " +
               string.Join("; ", result.Errors.Select(e => e.Description)));
        }

        return app;
    }

    public static async Task<IApplicationBuilder> SeedPatientUser([Required] this IApplicationBuilder app, IServiceProvider services, IConfiguration config)
    {
        ArgumentNullException.ThrowIfNull(services, nameof(services));

        using var scope = services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<PatientAppDbContext>();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<AppUserIdentity>>();

        ArgumentNullException.ThrowIfNull(context, nameof(context));

        var username = config["InitialUser:Username"];
        var password = config["InitialUser:Password"];
        var firstName = config["InitialUser:FirstName"];
        var lastName = config["InitialUser:LastName"];

        var isValidCreds = (!string.IsNullOrWhiteSpace(username) && !string.IsNullOrWhiteSpace(password));

        if (!isValidCreds) return app;

        var userToCreate = new AppUserIdentity
        {
            Email = username,
            NormalizedEmail = username!.ToUpper(),
            UserName = username,
            NormalizedUserName = username.ToUpper(),
            FirstName = firstName,
            LastName = lastName,
            DateOfBirth = DateTime.UtcNow,
            PhoneNumber = "+111111111111",
            EmailConfirmed = true,
            PhoneNumberConfirmed = true,
            IsActive = true,
            SecurityStamp = Guid.NewGuid().ToString("D")
        };

        var user = await userManager.FindByNameAsync(username);

        if (user is not null) return app;
        
        var result = await userManager.CreateAsync(userToCreate, password);
        
        if (!result.Succeeded)
        {
            throw new Exception("Failed to create initial user: " +
               string.Join("; ", result.Errors.Select(e => e.Description)));
        }

        return app;
    }
}
