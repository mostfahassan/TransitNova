using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using Serilog;
using System.Text.Json.Serialization;
using TransitNova.UI.Infrastructure.Mvc.Implementation;
using TransitNova.UI.Infrastructure.Mvc.Interface;
using TransitNovaUI.BusinessLayer;

namespace TransitNova.UI
{
    public static class Dependencies
    {
        public static IServiceCollection AddUIDependencies(this IServiceCollection services)
        {
            services.AddControllersWithViews();
            services.AddHttpContextAccessor();
            services.AddDistributedMemoryCache();
            services.AddSession(options =>
            {
                options.Cookie.Name = "TransitNova.Session";
                options.Cookie.HttpOnly = true;
                options.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;
                options.Cookie.SameSite = SameSiteMode.Lax;
                options.IdleTimeout = TimeSpan.FromHours(8);
            });
            services.AddUiAuthentication();
            services.AddUiAuthorization();
            services.AddMvcBackendServices();
            services.AddTransitNovaApiClients();

            return services;
        }

        public static IServiceCollection AddUiAuthentication(this IServiceCollection services)
        {
            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = CookieAuthenticationDefaults.AuthenticationScheme;
            })
             .AddCookie(options =>
             {
                 options.LoginPath = "/AccountArea/Account/Login";
                 options.LogoutPath = "/AccountArea/Account/Logout";
                 options.AccessDeniedPath = "/AccountArea/Account/AccessDenied";

                 options.Cookie.Name = "TransitNovaAuthCookie";
                 options.Cookie.HttpOnly = true;
                 options.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;
                 options.Cookie.SameSite = SameSiteMode.Lax;

                 options.ExpireTimeSpan = TimeSpan.FromDays(10);
                 options.SlidingExpiration = true;
             });
            return services;
        }

        public static IServiceCollection AddUiAuthorization(this IServiceCollection services)
        {
            services.AddAuthorization(options =>
            {
                options.AddPolicy("AdminPolicy", policy => policy.RequireRole("Admin"));
                options.AddPolicy("CarrierPolicy", policy => policy.RequireRole("Carrier"));
                options.AddPolicy("OperationManagerPolicy", policy => policy.RequireRole("OperationManager"));
                options.AddPolicy("WarehouseManagerPolicy", policy => policy.RequireRole("WarehouseManager"));
                options.AddPolicy("UserPolicy", policy => policy.RequireRole("User"));
            });
            return services;
        }

        public static IServiceCollection AddMvcBackendServices(this IServiceCollection services)
        {
            services.AddScoped<IBackendApiInvoker, BackendApiInvoker>();
            services.AddScoped<IUiAuthSessionService, UiAuthSessionService>();
            services.AddScoped<IRoleHomeResolver, RoleHomeResolver>();
            services.AddScoped<IIdempotencyKeyFactory, IdempotencyKeyFactory>();
            services.AddScoped<IWarehouseContextService, WarehouseContextService>();

            return services;
        }

        public static IHostBuilder AddSerilog(this IHostBuilder hostBuilder)
        {
            return hostBuilder.UseSerilog((context, configuration) =>
            {
                configuration.ReadFrom.Configuration(context.Configuration);
                configuration.Enrich.FromLogContext().WriteTo.Console(
                outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] [{CorrelationId}] {Message:lj}{NewLine}{Exception}");
            });
        }

        public static IServiceCollection AddOpenTelemetryServices(this IServiceCollection services)
        {
            services.AddOpenTelemetry()
                .ConfigureResource(resource => resource.AddService("transitnova-ui"))
                .WithTracing(tracing =>
                {
                    tracing
                        .AddAspNetCoreInstrumentation()
                        .AddHttpClientInstrumentation();
                    tracing.AddOtlpExporter();
                });

            return services;
        }

        public static IServiceCollection AddJsonSerializer(this IServiceCollection service)
        {
            service.AddControllers()
            .AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
            });
            return service;
        }

        public static IServiceCollection AddApiVersioning(this IServiceCollection service)
        {
            service.AddApiVersioning(options =>
            {
                options.AssumeDefaultVersionWhenUnspecified = true;
                options.DefaultApiVersion = new ApiVersion(1, 0);
                options.ReportApiVersions = true;
            });
            return service;
        }
    }
}
