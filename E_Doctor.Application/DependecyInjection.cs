using E_Doctor.Application.Interfaces.Features.Admin.Dashboard;
using E_Doctor.Application.Interfaces.Features.Admin.Settings;
using E_Doctor.Application.Interfaces.Features.Common;
using E_Doctor.Application.Interfaces.Features.Diagnosis;
using E_Doctor.Application.Interfaces.Features.Patient.Dashboard;
using E_Doctor.Application.Services.Features.Admin.Dashboard;
using E_Doctor.Application.Services.Features.Admin.Settings;
using E_Doctor.Application.Services.Features.Common;
using E_Doctor.Application.Services.Features.Diagnosis;
using E_Doctor.Application.Services.Features.Patient.Dashboard;
using Microsoft.Extensions.DependencyInjection;

namespace E_Doctor.Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddAdminApplication(this IServiceCollection services)
        {
            services.RegisterAdminServices();
            
            return services;
        } 

        private static IServiceCollection RegisterAdminServices(this IServiceCollection services)
        {
            services.AddScoped<ICommonService, CommonService>();
            services.AddScoped<ISymptomService, SymptomService>();
            services.AddScoped<IRuleManagementService, RuleManagementService>();
            services.AddScoped<IDiagnosisService, DiagnosisService>();
            services.AddScoped<IUserManagerService, UserManagerService>();
            services.AddScoped<IDashboardService, DashboardService>();

            return services;
        }

        public static IServiceCollection AddPatientApplication(this IServiceCollection services)
        {
            services.RegisterPatientServices();

            return services;
        }

        private static IServiceCollection RegisterPatientServices(this IServiceCollection services)
        {
            services.AddScoped<ICommonService, CommonService>();
            services.AddScoped<IDiagnosisService, DiagnosisService>();
            services.AddScoped<IRuleManagementService, RuleManagementService>();
            services.AddScoped<IUserManagerService, UserManagerService>();
            services.AddScoped<IPatientDashboardService, PatientDashboardService>();

            return services;
        }

    }
}
