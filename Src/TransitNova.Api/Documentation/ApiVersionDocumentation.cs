using Microsoft.AspNetCore.OpenApi;
using Microsoft.OpenApi;

namespace TransitNova.Api.Documentation
{
    internal sealed class ApiVersionDocumentation : IOpenApiDocumentTransformer
    {
        public Task TransformAsync(OpenApiDocument document, OpenApiDocumentTransformerContext context, CancellationToken cancellationToken)
        {
            var version = context.DocumentName;
            document.Info.Version = version;
            document.Info.Title = $"Project Api Version => {version}";

            return Task.CompletedTask;
        }
    }
}
