using TransitNova.UI;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddUIDependencies();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/AccountArea/Errors/ServerError");
    app.UseHsts();
}

app.UseStatusCodePagesWithReExecute("/AccountArea/Errors/StatusCode", "?statusCode={0}");
app.UseHttpsRedirection();
app.UseRouting();
app.UseSession();
app.UseAuthentication();
app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "landing-root",
    pattern: "",
    defaults: new { area = "LandingArea", controller = "Home", action = "Index" })
    .WithStaticAssets();

app.MapControllerRoute(
    name: "areas",
    pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

app.Run();
