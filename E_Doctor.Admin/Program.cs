using E_Doctor.Application;
using E_Doctor.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
//builder.Services.AddSession();

builder.Services
    .AddAdminInfrastructure(builder.Configuration)
    .AddAdminApplication();

var app = builder.Build();

// seeder
using var scope = app.Services.CreateScope();
var services = scope.ServiceProvider;
await app.SeedAdminUser(services, builder.Configuration);

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
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
