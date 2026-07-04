
using Serilog.Context;
namespace TransitNova.Api.CustomMiddlewares
{
    public class CorrelationIDMiddleware : IMiddleware
    {
        public async Task  InvokeAsync(HttpContext context, RequestDelegate next)
        {
            var correlationID = context.Request.Headers["X-Correlation-ID"].FirstOrDefault() ?? Guid.CreateVersion7().ToString();
            context.TraceIdentifier = correlationID;
            context.Response.Headers["X-Correlation-ID"] = correlationID;
            using (LogContext.PushProperty("CorrelationId", correlationID)) 
            await next(context);
        }
        
    }
}
