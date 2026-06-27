using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.OpenApi;
using Microsoft.OpenApi;

namespace TransitNova.Api.Documentation
{
    internal sealed class ApiOperationSecuritySchemeDocumentation : IOpenApiOperationTransformer
    {
        private const string SchemeId = JwtBearerDefaults.AuthenticationScheme;
        public Task TransformAsync(OpenApiOperation operation, OpenApiOperationTransformerContext context, CancellationToken cancellationToken)
        {
            if (context.Description.ActionDescriptor.EndpointMetadata.OfType<IAuthorizeData>().Any())
            {
                operation.Security ??= [];

                var requirement = new OpenApiSecurityRequirement
                {
                    { new OpenApiSecuritySchemeReference(SchemeId), [] }
                };

                operation.Security.Add(requirement);
            }

            return Task.CompletedTask;
        }
    }
}
