using YT_Downloader.Components;
using YT_Downloader.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers(); // Register DownloadController as a controller
// Register the hosted service with dependency injection
builder.Services.AddSingleton<ICookieManager, CookieManager>();
builder.Services.AddScoped<DownloadHandler>(); // Register DownloadHandler as a scoped service
// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();
var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();
app.UseAntiforgery();

app.MapControllers(); // Map controller routes

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
