using Application.Behaviours;
using Application.Extensions.Values;
using Application.Statics.Configurations;
using Application.Statics.Externals;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System.Reflection;
using System.Security.Claims;
using System.Text;

namespace Application
{
    public static class ServiceExtension
    {
        public static void AddApplicationServices(this IServiceCollection services, IConfiguration configuration)
        {
            configuration.ConfigureSettings();

            services.AddAutoMapper(Assembly.GetExecutingAssembly());
            services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());
            services.AddMediatR(conf =>
            {
                conf.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly());
            });

            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(UnhandledExceptionBehaviour<,>));
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehaviour<,>));
        }

        private static void ConfigureSettings(this IConfiguration configuration)
        {
            configuration.AddApiAuthSettings();
            configuration.AddDbSettings();
            configuration.AddSwaggerSettings();
            configuration.AddExternalApiSettings();
        }

        private static void AddApiAuthSettings(this IConfiguration configuration)
        {

            ApiAuthSettings.ApiKey = configuration["ApiAuthSettings:APIKEY"].ThrowIfNullOrEmpty();
            ApiAuthSettings.CorsPolicyName = configuration["ApiAuthSettings:CorsPolicyName"].ThrowIfNullOrEmpty();
        }

        private static void AddDbSettings(this IConfiguration configuration)
        {

            DbSettings.DefaultConnection = configuration.GetConnectionString("defaultConnection").ThrowIfNullOrEmpty();
        }

        private static void AddSwaggerSettings(this IConfiguration configuration)
        {
            SwaggerSettings.IsSwaggerEnabled = configuration.GetValue<bool>("SwaggerSettings:IsSwaggerEnabled");
        }

        private static void AddExternalApiSettings(this IConfiguration configuration)
        {
            ExternalApiUrl.PokemonApiUrl = configuration["ExternalApiUrl:PokemonApiUrl"].ThrowIfNullOrEmpty();
        }

        public static void AddAppAuthentication(this IServiceCollection services)
        {
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(opciones => opciones.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(ApiAuthSettings.ApiKey)),
                    ClockSkew = TimeSpan.Zero,
                    RoleClaimType = ClaimTypes.Role

                });
        }
    }
}
