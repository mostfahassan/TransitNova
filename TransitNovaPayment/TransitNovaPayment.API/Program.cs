using TransitNovaPayment.Api;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDependencies(builder.Configuration);
builder.Host.AddSerilog();

var app = builder.Build();

app.UseDependencies();

app.Run();

