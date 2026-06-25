using TransitNovaPayment.Api;
using TransitNovaPayment.API;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDependencies(builder.Configuration);
builder.Host.AddSerilog();

var app = builder.Build();

await DatabaseSyncronyzation.ApplyDatabaseMigrationsAsync(app);

app.UseDependencies();

app.Run();

