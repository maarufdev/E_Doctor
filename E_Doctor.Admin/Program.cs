using E_Doctor.Application;
using E_Doctor.Infrastructure;
using ElectronNET.API;
using ElectronNET.API.Entities;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services
    .AddAdminInfrastructure(builder.Configuration)
    .AddAdminApplication();


builder.WebHost.UseElectron(args);

var app = builder.Build();

// Seeder
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    await app.SeedAdminUser(services, builder.Configuration);
}

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

// Enable Electron.NET
if (HybridSupport.IsElectronActive)
{
    Task.Run(CreateElectronWindow);
    await app.RunAsync(); 
}
else
{
    app.Run(); 
}

static async Task CreateElectronWindow()
{
    Console.WriteLine("Creating Electron window...");

    var window = await Electron.WindowManager.CreateWindowAsync(
        new BrowserWindowOptions
        {
            Width = 1200,
            Height = 800,
            Show = true,// <-- Always show window immediately
            AutoHideMenuBar = true // hides the menu bar
        }
    );

    window.OnClosed += () =>
    {
        Electron.App.Quit();
    };
}
