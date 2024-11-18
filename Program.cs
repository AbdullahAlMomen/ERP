using Microsoft.AspNetCore.Builder;
using Serilog;
using Serilog.Sinks.SystemConsole.Themes;
var builder = WebApplication.CreateBuilder(args);
var env_name = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
var Configuration = new ConfigurationBuilder()
                       .SetBasePath(Directory.GetCurrentDirectory())
                       .AddJsonFile($"appsettings.{env_name}.json", optional: false, reloadOnChange: true)
                       .Build();
// Add services to the container.
string logpath = Configuration["FilePaths:LogFileRootDirectory"];
builder.Host.UseSerilog((hostContext, services, configuration) =>
{
    configuration.Enrich.FromLogContext()
   .MinimumLevel.Override("Microsoft.EntityFrameworkCore.Database.Command", Serilog.Events.LogEventLevel.Error)
   .MinimumLevel.Override("Microsoft.EntityFrameworkCore", Serilog.Events.LogEventLevel.Error)
   .WriteTo.Console(theme: AnsiConsoleTheme.Literate)
   .WriteTo.Debug(outputTemplate: DateTime.Now.ToString())
   .WriteTo.File(logpath + ".log", rollingInterval: RollingInterval.Hour, retainedFileCountLimit: null);
});

builder.Services.AddControllers();
builder.Services.AddHttpLogging(options =>
{
    options.LoggingFields = Microsoft.AspNetCore.HttpLogging.HttpLoggingFields.All;
});
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.MapOpenApi();
}

app.UseHttpsRedirection();
app.UseHttpLogging();

app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
