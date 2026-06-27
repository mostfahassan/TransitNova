using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.RateLimiting;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using Serilog;
using System.Text.Json.Serialization;
using System.Threading.RateLimiting;
using TransitNovaPayment.Busieness;
using TransitNovaPayment.Busieness.Common.Options;
using TransitNovaPayment.Infrastructure;
namespace TransitNovaPayment.Api
{
    public static class Dependencies
    {
        public static IServiceCollection AddDependencies(this IServiceCollection service, IConfiguration configuration)
        {
            service.AddOpenApi();
            service.AddPaymentSettingsConfiguration(configuration);
            service.AddBuisnessDependencies();
            service.AddInfrastructureDependencies();
            service.AddJsonSerializer();
            service.AddOpenTelemetryServices();
            service.AddAuthentication();
            service.AddAuthorization();
            service.AddCorsConfiguration();
            service.AddRateLimiting();
            service.AddCachingConfiguration();
            service.AddProblemDetailsService();
            service.AddApiVersioning();
            service.AddHealthChecks();

            return service;
        }


        // Middleware
        public static WebApplication UseDependencies(this WebApplication app)
        {
            if (app.Environment.IsDevelopment())
            {
                app.MapOpenApi();
            }
            app.UseExceptionHandler();
            app.UseHsts();
            app.UseHttpsRedirection();
            app.UseSerilogRequestLogging();
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseRateLimiter();
            app.MapHealthChecks("health");
            app.MapControllers();
            return app;
        }


        // Service Registeration
        // Logging
        public static IHostBuilder AddSerilog(this IHostBuilder hostBuilder)
        {
            return hostBuilder.UseSerilog((context, configuration) =>
            {
                configuration.ReadFrom.Configuration(context.Configuration);
            });
        }

        public static IServiceCollection AddPaymentSettingsConfiguration(this IServiceCollection service, IConfiguration configuration)
        {
            service.AddOptions<PaymentGatewaySettings>()
                .Bind(configuration.GetSection(PaymentGatewaySettings.SectionName))
                .Validate(settings => !string.IsNullOrWhiteSpace(settings.PrivateKey), "PaymentSettings:PrivateKey is required.")
                .ValidateOnStart();

            return service;
        }
        // Problem Details
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
        // Json Serializer

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

        public static IServiceCollection AddCorsConfiguration(this IServiceCollection service)
        {
            service.AddCors(options =>
            {
                options.AddPolicy("AllowMVC", policy =>
                {
                    policy
                        .AllowAnyOrigin()
                        .AllowAnyMethod()
                        .AllowAnyHeader();
                });
            });

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

                        await context.HttpContext.Response.WriteAsJsonAsync(proplemDetails, token);
                    }
                };
            });
            return service;
        }

        public static IServiceCollection AddOpenTelemetryServices(
               this IServiceCollection services)
        {
            services.AddOpenTelemetry()
                .ConfigureResource(resource => resource.AddService("transitnova-payment"))
                .WithTracing(tracing =>
                {
                    tracing
                        .AddAspNetCoreInstrumentation()
                        .AddHttpClientInstrumentation();
                    tracing.AddOtlpExporter();
                });

            return services;
        }
    }
}