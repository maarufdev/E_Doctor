using Microsoft.Extensions.DependencyInjection;

namespace E_Doctor.Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddAdminApplication(this IServiceCollection services)
        {
            //services.RegisterAdminServices();
            
            return services;
        } 

        public static IServiceCollection AddPatientApplication(this IServiceCollection services)
        {
            //services.RegisterPatientServices();

            return services;
        }
    }
}
