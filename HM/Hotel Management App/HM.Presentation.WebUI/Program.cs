using HM.Application;
using HM.Infrastructure;
using HM.Presentation.WebUI.StartupConfig;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddInfrastructureDependencyInjection(builder.Configuration);
builder.Services.AddApplicationDependencyInjection();

builder.Services.AddControllersWithViews();
builder.EnableRateLimiter();
builder.Services.AddSerilog(opts => opts.WriteTo.Console());

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
        "default",
        "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();


app.Run();

public partial class Program
{
}