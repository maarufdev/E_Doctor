using E_Doctor.Application;
using E_Doctor.Infrastructure;
using E_Doctor.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Force Kestrel to always use port 5000
builder.WebHost.UseUrls("http://localhost:5000");

// Add services to the container.
builder.Services.AddControllersWithViews();
//builder.Services.AddSession();

builder.Services
    .AddAdminInfrastructure(builder.Configuration)
    .AddAdminApplication();

var app = builder.Build();

// Seeder
using var scope = app.Services.CreateScope();
var services = scope.ServiceProvider;
await app.SeedAdminUser(services, builder.Configuration);

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseSession();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=UserDashboard}/{action=Index}/{id?}");

app.Run();
