using AdPlatformLocator.Application.Abstarctions;
using AdPlatformLocator.Application.Abstarctions.Services;
using AdPlatformLocator.Application.Services;
using AdPlatformLocator.Infrastructure.Persistence;

namespace AdPlatformLocator.Web.Configurations
{
    public static class DependicyInjectionConfigurations
    {
        public static void ConfigureDI(this WebApplicationBuilder builder)
        {


            builder.Services.AddScoped<IAdPlatformRepository, InMemoryAdPlatformRepository>();

            builder.Services.AddScoped<IAdPlatformService, AdPlatformService>();
        }
    }
}