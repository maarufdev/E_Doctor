using E_Doctor.Application.Interfaces.Features.Admin.Diagnosis;
using E_Doctor.Application.Interfaces.Features.Admin.Settings;
using E_Doctor.Application.Interfaces.Features.Common;
using E_Doctor.Application.Interfaces.Features.Patient.Diagnosis;
using E_Doctor.Application.Services.Admin.Diagnosis;
using E_Doctor.Application.Services.Admin.Settings;
using E_Doctor.Application.Services.Common;
using E_Doctor.Application.Services.Patient.Diagnosis;
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
            services.AddScoped<IPatientService, PatientService>();

            return services;
        }

    }
}
