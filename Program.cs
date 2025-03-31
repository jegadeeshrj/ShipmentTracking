using MongoDB.Driver;  // For MongoClient
using Microsoft.AspNetCore.Builder;  // For WebApplication
using Microsoft.Extensions.DependencyInjection;  // For AddScoped, AddSingleton
using Microsoft.Extensions.Hosting;  // For app.Run()
using Serilog;  // For Serilog ILogger
using Serilog.Extensions.Logging;  // For Serilog logging extensions

var builder = WebApplication.CreateBuilder(args);

// Configure Serilog
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .WriteTo.Console()
    .WriteTo.File("logs/shipment-{Date}.txt", rollingInterval: RollingInterval.Day)
    .CreateLogger();

// Add services
builder.Services.AddControllersWithViews();
builder.Services.AddSignalR();
builder.Services.AddSingleton<IMongoClient>(new MongoClient("mongodb://localhost:27017"));
builder.Services.AddScoped<IShipmentService, ShipmentService>();
builder.Services.AddScoped<IEmailService, EmailService>();
// Add Serilog logging
builder.Logging.ClearProviders();  // Remove default logging providers
builder.Logging.AddSerilog();  // Add Serilog as the logging provider

var app = builder.Build();

// Configure middleware
app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.MapHub<ShipmentHub>("/shipmentHub");

app.Run();

// Ensure logger is disposed when application shuts down
Log.CloseAndFlush();