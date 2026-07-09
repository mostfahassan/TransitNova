using TransitNova.Api;
var builder = WebApplication.CreateBuilder(args);
// Create services to the container.
#region Services
builder.Services.AddDependencies(builder.Configuration);
builder.Host.AddSerilog();
#endregion

var app = builder.Build();
await DatabaseSyncronyzation.ApplyDatabaseMigrationsAsync(app);
app.UseDependencies();
app.Run();




