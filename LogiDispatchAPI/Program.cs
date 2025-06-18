using LogiDispatchAPI.Extensions;
using LogiDispatchAPI.Tools;
using Application;
using Application.Statics.Configurations;
using Infrastructure;
using Microsoft.OpenApi.Models;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddApplicationServices(builder.Configuration);
builder.Services.AddInfrastructureServices();

// Add authentication
builder.Services.AddAppAuthentication();

builder.Services.AddControllers(options =>
{
    options.Conventions.Add(new SwaggerGroupByVersion());
})
    .AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
            });

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(
        c =>
        {
            c.SwaggerDoc("v1", new OpenApiInfo { Title = "LogiDispatchAPI", Version = "v1" });
            c.SwaggerDoc("v2", new OpenApiInfo { Title = "LogiDispatchAPI", Version = "v2" });

            c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                Name = "Authorization",
                Type = SecuritySchemeType.ApiKey,
                Scheme = "Bearer",
                BearerFormat = "JWT",
                In = ParameterLocation.Header
            });

            c.AddSecurityRequirement(new OpenApiSecurityRequirement
        {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id="Bearer",
                        }
                    }, Array.Empty<string>()
                }
        });
        }
        );


builder.Services.AddCors(options =>
{
    options.AddPolicy(ApiAuthSettings.CorsPolicyName, builder =>
    {
        builder.WithOrigins("*").AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader();
        builder.WithExposedHeaders("content-disposition");
        builder.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader();
        builder.SetIsOriginAllowed(origin => true);
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (SwaggerSettings.IsSwaggerEnabled)
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "LogiDispatchAPI v1");
        c.SwaggerEndpoint("/swagger/v2/swagger.json", "LogiDispatchAPI v2");
    });
}

app.UseCors(ApiAuthSettings.CorsPolicyName);

app.UseHttpsRedirection();

app.UseErrorHandlingMiddleware();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();
