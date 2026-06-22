
using TransitNova.Api;

var builder = WebApplication.CreateBuilder(args);
// Create services to the container.
#region Services
builder.Services.AddDependencies(builder.Configuration);
builder.Host.AddSerilog();
#endregion

var app = builder.Build();
app.UseDependencies();
app.Run();

public partial class Program;
