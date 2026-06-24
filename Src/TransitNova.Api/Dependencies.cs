
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.RateLimiting;
using Serilog;
using System.Text.Json.Serialization;
using System.Threading.RateLimiting;
using TransitNova.Api.AuthorizationResource.Handler;
using TransitNova.Api.AuthorizationResource.Requirement;
using TransitNova.Api.Exceptions;
using TransitNova.BusinessLayer;
using TransitNova.Domain.Contracts.Permissions;
using TransitNova.InfraStructure.Common.NotificationService.NotificationHubService;
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
            service.AddInfraStructureService(configuration).AddInBusinessService();
            service.AddJsonSerializer();
            service.AddRateLimiting();
            service.AddCachingConfiguration();
            service.AddExceptionHandler<GlobalExceptionHandler>();
            service.AddProblemDetailsService();
            service.AddApiVersioning();
            service.AddAuthorizationBehavior();
            return service;
        }


        // â”€â”€ Middleware
        public static WebApplication UseDependencies(this WebApplication app)
        {
            if (app.Environment.IsDevelopment())
            {
                app.MapOpenApi();
            }
            app.UseExceptionHandler(); 
            app.UseHsts();
            app.UseHttpsRedirection();
            app.UseCors("AllowMVC");
            app.UseSerilogRequestLogging();
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseRateLimiter();
            app.MapHub<NotificationHub>("/hubs/notifications");
            app.MapHealthChecks("health");
            app.MapControllers();
            return app;
        }


        //Service Registeration
        // â”€â”€ Logging
        public static IHostBuilder AddSerilog(this IHostBuilder hostBuilder)
        {
            return hostBuilder.UseSerilog((context, configuration) =>
            {
                configuration.ReadFrom.Configuration(context.Configuration);
            });
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
            service.Configure<JwtSettings>(configuration.GetSection("JWT"));
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
                .AddPolicy("RefreshTokenOwner",
                policy =>
                {
                    policy.RequireAuthenticatedUser();
                    policy.AddRequirements(new RefreshTokenAuthenticationRequirement());
                });
            service.AddAuthorizationBuilder()
                .AddPolicy("IsTokenOwner",
                policy =>
                {
                    policy.RequireAuthenticatedUser();
                    policy.AddRequirements(new RefreshTokenOwnershipRequirement());
                });

            return service;
        }
        
       public static IServiceCollection RegisterApiService(this IServiceCollection service)
        {
            service.AddScoped<IAuthorizationHandler, ShipmentOwnerHandler>();

            service.AddScoped<IAuthorizationHandler, CarrierOwnerHandler>();

            service.AddScoped<IAuthorizationHandler, CompletedProfileHandler>();
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


    }
}
