using E_Doctor.Application.Interfaces.Features.Common;
using E_Doctor.Application.Interfaces.Features.Diagnosis;
using E_Doctor.Application.Interfaces.Features.Settings;
using E_Doctor.Application.Services.Common;
using E_Doctor.Application.Services.Diagnosis;
using E_Doctor.Application.Services.Settings;
using Microsoft.Extensions.DependencyInjection;

namespace E_Doctor.Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            services.RegisterServices();
            
            return services;
        } 

        private static IServiceCollection RegisterServices(this IServiceCollection services)
        {
            services.AddScoped<ICommonService, CommonService>();
            services.AddScoped<ISymptomService, SymptomService>();
            services.AddScoped<IRuleManagementService, RuleManagementService>();
            services.AddScoped<IDiagnosisService, DiagnosisService>();

            return services;
        }

    }
}
