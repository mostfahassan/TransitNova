using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.OpenApi;
using Microsoft.OpenApi;

namespace TransitNova.Api.Documentation
{
    internal sealed class ApiSecuritySchemeDocumentation : IOpenApiDocumentTransformer
    {
        private const string SchemeId = JwtBearerDefaults.AuthenticationScheme;

        public Task TransformAsync(OpenApiDocument document, OpenApiDocumentTransformerContext context, CancellationToken cancellationToken)
        {
            document.Components ??= new();
            document.Components.SecuritySchemes ??= new Dictionary<string, IOpenApiSecurityScheme>();
            document.Components.SecuritySchemes[SchemeId] = new OpenApiSecurityScheme
            {
                Type = SecuritySchemeType.Http,
                Scheme = "bearer",
                BearerFormat = "JWT",
                In = ParameterLocation.Header,
                Description = "Enter JWT Bearer token",
                Name = "Authorization"
            };

            return Task.CompletedTask;
        }
    }
}
