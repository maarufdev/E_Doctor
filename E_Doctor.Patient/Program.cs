using E_Doctor.Infrastructure;
using E_Doctor.Infrastructure.ApplicationBackgroundServices;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services
    .AddPatientInfrastructure(builder.Configuration);

builder.Services.AddHostedService<InActiveUserArchiverService>();

var app = builder.Build();
// seeder
using var scope = app.Services.CreateScope();
var services = scope.ServiceProvider;
await app.SeedPatientUser(services, builder.Configuration);

// Con
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
    pattern: "{controller=Dashboard}/{action=Index}/{id?}");

app.Run();
