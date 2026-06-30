
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.RateLimiting;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using Scalar.AspNetCore;
using Serilog;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.RateLimiting;
using TransitNova.Api.AuthorizationResource.Handler;
using TransitNova.Api.AuthorizationResource.Requirement;
using TransitNova.Api.CustomMiddlewares;
using TransitNova.Api.Documentation;
using TransitNova.Api.Exceptions;
using TransitNova.BusinessLayer;
using TransitNova.BusinessLayer.Options;
using TransitNova.Domain.Contracts.Permissions;
using TransitNova.InfraStructure.SignalR.NotificationHubService;
using TransitNova.InfraStructure.ServiceRegistration;
using TransitNova.InfraStructure.Token;
namespace TransitNova.Api
{
    public static class Dependencies
    {

        // â”€â”€ Services
        public static IServiceCollection AddDependencies(this IServiceCollection service, IConfiguration configuration)
        {
            service.RegisterApiService();
            service.AddOpenApi();
            service.AddJWTConfiguration(configuration);
            service.AddPaymentSettingsConfiguration(configuration);
            service.AddInfraStructureService(configuration).AddInBusinessService();
            service.AddJsonSerializer();
            service.AddOpenTelemetryServices();
            service.AddRateLimiting();
            service.AddOpenApiDocumentation();
            service.AddCachingConfiguration();
            service.AddExceptionHandler<GlobalExceptionHandler>();
            service.AddProblemDetailsService();
            service.AddApiVersioning();
            service.AddAuthorizationBehavior();
            service.AddHttpClient();
            return service;
        }


        // â”€â”€ Middleware
        public static WebApplication UseDependencies(this WebApplication app)
        {
           
            app.UseExceptionHandler(); 
            app.UseHsts();
            app.UseHttpsRedirection();
            app.UseCors("AllowMVC");
            app.UseSerilogRequestLogging();
            app.UseMiddleware<CorrelationIDMiddleware>();
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseRateLimiter();
            app.MapHub<NotificationHub>("/hubs/notifications");
            app.MapHealthChecks("health");
            app.MapControllers();
            if (app.Environment.IsDevelopment())
            {
                app.MapOpenApi();

                app.MapScalarApiReference();
            }

            return app;
        }


        //Service Registeration
        public static IHostBuilder AddSerilog(this IHostBuilder hostBuilder)
        {
            return hostBuilder.UseSerilog((context, configuration) =>
            {
                configuration.ReadFrom.Configuration(context.Configuration);
                configuration.Enrich.FromLogContext().WriteTo.Console(
                outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] [{CorrelationId}] {Message:lj}{NewLine}{Exception}");
            });
       
        }

        public static IServiceCollection AddOpenTelemetryServices(
                  this IServiceCollection services)
        {
            services.AddOpenTelemetry()
                .ConfigureResource(resource => resource.AddService("transitnova_api"))
                .WithTracing(tracing =>
                {
                    tracing
                        .AddAspNetCoreInstrumentation()
                        .AddHttpClientInstrumentation();
                    tracing.AddOtlpExporter();
                });
              
            return services;
        }

        
        // --- Problem Details
        public static IServiceCollection AddProblemDetailsService(this IServiceCollection service)
        {
            service.AddProblemDetails(config =>
            {
                config.CustomizeProblemDetails = (context) =>
                {
                    context.ProblemDetails.Instance = $"{context.HttpContext.Request.Method} {context.HttpContext.Request.Path}";
                    var environment = context.HttpContext.RequestServices.GetRequiredService<IWebHostEnvironment>();
                    if (environment.IsDevelopment())
                    {
                        context.ProblemDetails.Extensions["traceId"] =
                            context.HttpContext.TraceIdentifier;
                    }
                };
            });
            return service;

        }
        // ---- Json Serializer

        public static IServiceCollection AddJsonSerializer(this IServiceCollection service)
        {
            service.AddControllers()
            .AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.Converters.Add(
                    new JsonStringEnumConverter()
                );
            });
            return service;
        }
        // Add JWt Configuration
        public static IServiceCollection AddJWTConfiguration(this IServiceCollection service, IConfiguration configuration)
        {
            service.AddOptions<JwtSettings>()
                .Bind(configuration.GetSection("JWT"))
                .Validate(settings => !string.IsNullOrWhiteSpace(settings.Key), "JWT:Key is required.")
                .Validate(settings => !string.IsNullOrWhiteSpace(settings.Issuer), "JWT:Issuer is required.")
                .Validate(settings => !string.IsNullOrWhiteSpace(settings.Audience), "JWT:Audience is required.")
                .Validate(settings => Encoding.UTF8.GetByteCount(settings.Key ?? string.Empty) >= 48, "JWT:Key must be at least 48 bytes for HS384 signing.")
                .ValidateOnStart();
            return service;
        }

        public static IServiceCollection AddPaymentSettingsConfiguration(this IServiceCollection service, IConfiguration configuration)
        {
            service.AddOptions<PaymentSettings>()
                .Bind(configuration.GetSection(PaymentSettings.SectionName))
                .Validate(settings => !string.IsNullOrWhiteSpace(settings.PublicKey), "PaymentSettings:PublicKey is required.")
                .Validate(settings => Uri.TryCreate(settings.BaseUrl, UriKind.Absolute, out var uri) && uri.Scheme is "http" or "https", "PaymentSettings:BaseUrl must be an absolute HTTP(S) URL.")
                .ValidateOnStart();

            return service;
        }

        public static IServiceCollection AddCachingConfiguration(this IServiceCollection service)
        {
             service.AddMemoryCache(config =>
             {
                 config.SizeLimit = 130;
                 config.ExpirationScanFrequency = TimeSpan.FromMinutes(5);
             });
             return service;
        }

        //------ Api Versioning

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


        //--Add Uthorization Behavior

        public static IServiceCollection AddAuthorizationBehavior(this IServiceCollection service)
        {
            service.AddAuthorizationBuilder()
                    .AddPolicy(UserPermissions.ShipmentOwner,
                        policy =>
                        {
                            policy.RequireAuthenticatedUser();
                            policy.AddRequirements(new ShipmentOwnerRequirement());
                        });
      

            service.AddAuthorizationBuilder()
                   .AddPolicy(CarrierPermissions.IsCarrierOwner,
                     policy =>
                     {
                         policy.RequireAuthenticatedUser();
                         policy.AddRequirements(new CarrierOwnerRequirement());

                     });

            service.AddAuthorizationBuilder()
                .AddPolicy(CarrierPermissions.HasCompletedProfile,
                policy =>
                {
                    policy.RequireAuthenticatedUser();
                    policy.AddRequirements(new CompletedProfileRequirement());
                });
                
            service.AddAuthorizationBuilder()
                .AddPolicy("IsTokenOwner",
                policy =>
                {
                    policy.RequireAuthenticatedUser();
                    policy.AddRequirements(new RefreshTokenOwnershipRequirement());
                });

            service.AddAuthorizationBuilder()
                .AddPolicy(WarehouseManagerPermissions.IsWarehouseManager,
                policy =>
                {
                    policy.RequireAuthenticatedUser();
                    policy.AddRequirements(new IsWarehouseManagerRequirement());
                });

            return service;
        }
        
       public static IServiceCollection RegisterApiService(this IServiceCollection service)
        {
            service.AddHttpContextAccessor();
            service.AddTransient<CorrelationIDMiddleware>();
            service.AddScoped<IAuthorizationHandler, ShipmentOwnerHandler>();
            service.AddScoped<IAuthorizationHandler, CarrierOwnerHandler>();
            service.AddScoped<IAuthorizationHandler, CompletedProfileHandler>();
            service.AddScoped<IAuthorizationHandler, IsWarehouseManagerRequirementHandler>();
            service.AddScoped<IAuthorizationHandler, TokenOwnerHandler>();
            return service;
        }

        public static IServiceCollection AddRateLimiting(this IServiceCollection service)
        {
            service.AddRateLimiter(options =>
            {
                options.AddFixedWindowLimiter("DefaultRateLimiter", options =>
                {
                    options.PermitLimit = 100;
                    options.Window = TimeSpan.FromMinutes(1);
                    options.QueueLimit = 50;
                    options.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
                });

                options.AddPolicy
                ("CommandsLimiter", context =>
                  RateLimitPartition.GetSlidingWindowLimiter(
                      partitionKey: context.User.Identity?.Name ?? context.Connection.RemoteIpAddress?.ToString(),
                      factory: _ => new SlidingWindowRateLimiterOptions
                      {
                          PermitLimit = 20,
                          QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
                          QueueLimit = 50,
                          Window = TimeSpan.FromMinutes(1),
                          SegmentsPerWindow = 6,
                      })
                );

                options.OnRejected = async (context, token) =>
                {
                    if (token.IsCancellationRequested) return;

                    if (context.Lease.TryGetMetadata(MetadataName.RetryAfter, out TimeSpan retryAfter))
                    {
                        context.HttpContext.Response.Headers.RetryAfter = ((int)Math.Ceiling(retryAfter.TotalSeconds)).ToString();
                        var proplemDetailsFactory = context.HttpContext.RequestServices.GetRequiredService<ProblemDetailsFactory>();
                        var proplemDetails = proplemDetailsFactory.CreateProblemDetails(context.HttpContext,
                              StatusCodes.Status429TooManyRequests,
                              "Too Many Requests",
                              detail: $"Too Many Requests, Please Try Again After {retryAfter.TotalSeconds} From Now");

                        await context.HttpContext.Response.WriteAsJsonAsync(proplemDetails,token);
                    }
                };
            });
            return service;
        }

        public static IServiceCollection AddOpenApiDocumentation(this IServiceCollection services)
        {
            services.AddOpenApi(options =>
            {
                options.AddDocumentTransformer<ApiVersionDocumentation>();

                options.AddDocumentTransformer<ApiSecuritySchemeDocumentation>();
                options.AddOperationTransformer<ApiOperationSecuritySchemeDocumentation>();
            });



            return services;
        }

    }
}
