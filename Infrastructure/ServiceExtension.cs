using Application.Attributes.Services;
using Application.Contracts.Persistence.Common.BaseRepository;
using Application.Contracts.Services.AuthServices;
using Application.Contracts.Services.ExternalApiServices;
using Application.Extensions.Attributes;
using Application.Statics.Configurations;
using Infrastructure.DbContexts;
using Infrastructure.Repositories.Common.BaseRepository;
using Infrastructure.Services.AuthServices;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace Infrastructure
{
    public static class ServiceExtension
    {
        private const string BaseApiServiceName = nameof(IBaseApiService);
        private const string DisposableInterfaceName = nameof(IDisposable);
        public static void AddInfrastructureServices(this IServiceCollection services)
        {
            ConfigureDatabase(services);
            ConfigureHttpContextAccessor(services);
            RegisterRepositories(services);
            RegisterServices(services);
            RegisterExternalApiServices(services);
        }

        private static void ConfigureDatabase(IServiceCollection services)
        {
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(DbSettings.DefaultConnection)
            );
        }

        private static void ConfigureHttpContextAccessor(IServiceCollection services)
        {
            services.AddHttpContextAccessor();
            services.AddHttpClient();
        }

        private static void RegisterRepositories(IServiceCollection services)
        {
            services.AddScoped(typeof(IBaseRepository<>), typeof(BaseRepository<>));
            services.RegisterServicesDecoratedWithRegisterServiceAttribute();
        }

        private static void RegisterServices(IServiceCollection services)
        {
            services.AddTransient<IAuthenticatedUserService, AuthenticatedUserService>();
        }

        private static void RegisterExternalApiServices(this IServiceCollection services)
        {
            var types = AttributesExtension.GetTypesDecoratedWith<RegisterExternalServiceAttribute>();

            foreach (var type in types)
            {
                RegisterExternalService(services, type);
            }
        }

        private static void RegisterExternalService(IServiceCollection services, Type type)
        {
            var attribute = type.GetCustomAttribute<RegisterExternalServiceAttribute>();
            var interfaceType = type.GetInterfaces().First(x => x.Name != BaseApiServiceName && x.Name != DisposableInterfaceName);

            services.AddHttpClient(type.Name)
                .ConfigureHttpClient(client => client.BaseAddress = new Uri(RegisterExternalServiceAttribute.BaseAddress))
                .AddTypedClient((cl, sp) => Convert.ChangeType(sp.GetRequiredService(type), interfaceType));
            services.Add(new ServiceDescriptor(interfaceType, type, attribute!.LifeTime));
        }

        private static void RegisterServicesDecoratedWithRegisterServiceAttribute(this IServiceCollection services)
        {
            var types = AttributesExtension.GetTypesDecoratedWith<RegisterServiceAttribute>();

            foreach (var type in types)
            {
                RegisterService(services, type);
            }
        }

        private static void RegisterService(IServiceCollection services, Type type)
        {
            var attribute = type.GetCustomAttribute<RegisterServiceAttribute>();
            var interfaceType = type.GetInterfaces().FirstOrDefault()!;
            services.Add(new ServiceDescriptor(interfaceType, type, attribute!.LifeTime));
        }
    }
}
