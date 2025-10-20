using E_Doctor.Infrastructure.Configurations;
using E_Doctor.Infrastructure.Constants;
using E_Doctor.Infrastructure.Data;
using E_Doctor.Infrastructure.Identity;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.ComponentModel.DataAnnotations;

namespace E_Doctor.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddAdminInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services
            //.AddDbContext<AppDbContext>(options => options.UseSqlite(configuration.GetConnectionString("DefaultConnection")))
            .AddMSSQL(configuration)
            .AddAdminCustomIdentityServices();

        return services;
    }

    private static IServiceCollection AddMSSQL(this IServiceCollection services, IConfiguration configuration)
    {
        MSSQLConfig mssqlConfig = new();
        configuration.Bind("MSSQLConnectionStrings", mssqlConfig);

        services.AddDbContext<AppDbContext>(options =>
        {
            options.UseSqlServer(
                    $"Server={mssqlConfig.DataSource};Database={mssqlConfig.Database};User Id={mssqlConfig.Username};Password={mssqlConfig.Password};TrustServerCertificate=True;",
                    builder =>
                    {
                        builder.EnableRetryOnFailure(5, TimeSpan.FromSeconds(10), null);
                    }
                );
        });

        return services;
    }

    public static IServiceCollection AddPatientInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services
            //.AddDbContext<PatientAppDbContext>(options => options.UseSqlite(configuration.GetConnectionString("DefaultConnection")))
            .AddMSSQL(configuration)
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
            .AddEntityFrameworkStores<AppDbContext>()
            .AddDefaultTokenProviders();

        // We configure the cookie authentication settings first.
        services.ConfigureApplicationCookie(options =>
        {
            options.LoginPath = "/Account/Login";
            options.AccessDeniedPath = "/Account/Login";

            // 👇 Force cookie to behave like a "session cookie"
            options.Cookie.HttpOnly = true;
            options.Cookie.IsEssential = true;
            options.Cookie.SecurePolicy = CookieSecurePolicy.Always;

            // Expiration settings
            options.ExpireTimeSpan = TimeSpan.FromDays(30); // valid only for 30 mins
            options.SlidingExpiration = false; // lifetime won’t refresh on activity

            // 👇 This makes the cookie disappear when browser closes
            options.Cookie.MaxAge = null;
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
            .AddEntityFrameworkStores<AppDbContext>()
            .AddDefaultTokenProviders();

        // We configure the cookie authentication settings first.
        services.ConfigureApplicationCookie(options =>
        {
            options.LoginPath = "/Account/Login";
            options.AccessDeniedPath = "/Account/Login";

            // 👇 Force cookie to behave like a "session cookie"
            options.Cookie.HttpOnly = true;
            options.Cookie.IsEssential = true;
            options.Cookie.SecurePolicy = CookieSecurePolicy.Always;

            // Expiration settings
            options.ExpireTimeSpan = TimeSpan.FromDays(30); // valid only for 30 mins
            options.SlidingExpiration = false; // lifetime won’t refresh on activity
        });

        services.AddSession(o =>
        {
            o.IdleTimeout = TimeSpan.FromMinutes(100);
            o.Cookie.HttpOnly = true;
            o.Cookie.IsEssential = true;
        });

        return services;
    }

    public static async Task<IApplicationBuilder> SeedRoleAsync([Required] this IApplicationBuilder app, IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();
        var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole<int>>>();
        
        string[] roles =
        {
        RoleConstants.Admin,
        RoleConstants.Patient,
        };


        foreach (var roleName in roles)
        {
            if (!await roleManager.RoleExistsAsync(roleName))
            {
                await roleManager.CreateAsync(new IdentityRole<int>(roleName));
            }
        }

        return app;
    }

    public static async Task<IApplicationBuilder> SeedAdminUser([Required] this IApplicationBuilder app, IServiceProvider services, IConfiguration config)
    {
        ArgumentNullException.ThrowIfNull(services, nameof(services));

        using var scope = services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<AppUserIdentity>>();
        var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole<int>>>();

        ArgumentNullException.ThrowIfNull(context, nameof(context));

        if (!await roleManager.RoleExistsAsync(RoleConstants.Admin))
        {
            await roleManager.CreateAsync(new IdentityRole<int>(RoleConstants.Admin));
        }

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

        var addedToRole = await userManager.AddToRoleAsync(userToCreate, RoleConstants.Admin);

        if (addedToRole.Succeeded)
        {
            Console.WriteLine($"[SUCCESS] User '{username}' created and added to role '{RoleConstants.Admin}'.");
        }
        else
        {
            Console.WriteLine($"[WARN] User created but failed to add to role: {string.Join(", ", addedToRole.Errors.Select(e => e.Description))}");
        }

        return app;
    }

    public static async Task<IApplicationBuilder> SeedPatientUser([Required] this IApplicationBuilder app, IServiceProvider services, IConfiguration config)
    {
        ArgumentNullException.ThrowIfNull(services, nameof(services));

        using var scope = services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<AppUserIdentity>>();
        var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole<int>>>();

        ArgumentNullException.ThrowIfNull(context, nameof(context));


        if (!await roleManager.RoleExistsAsync(RoleConstants.Patient))
        {
            await roleManager.CreateAsync(new IdentityRole<int>(RoleConstants.Patient));
        }

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

        var addedToRole = await userManager.AddToRoleAsync(userToCreate, RoleConstants.Patient);

        if (addedToRole.Succeeded)
        {
            Console.WriteLine($"[SUCCESS] User '{username}' created and added to role '{RoleConstants.Patient}'.");
        }
        else
        {
            Console.WriteLine($"[WARN] User created but failed to add to role: {string.Join(", ", addedToRole.Errors.Select(e => e.Description))}");
        }

        return app;
    }
}
