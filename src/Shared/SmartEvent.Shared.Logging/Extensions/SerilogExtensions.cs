using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Events;

namespace SmartEvent.Shared.Logging.Extensions;

public static class SerilogExtensions
{
    public static WebApplicationBuilder AddSmartEventLogging(
        this WebApplicationBuilder builder,
        string serviceName)
    {
        builder.Host.UseSerilog((context, services, configuration) =>
        {
            // base config from appsettings.json
            configuration
                .ReadFrom.Configuration(context.Configuration)
                .Enrich.FromLogContext()
                .Enrich.WithMachineName()
                .Enrich.WithEnvironmentName()
                .Enrich.WithThreadId()
                .Enrich.WithProperty("ServiceName", serviceName);

            // different config for different environments
            if (context.HostingEnvironment.IsDevelopment())
            {
                ConfigureDevelopmentLogging(configuration, context.Configuration);
            }
            else
            {
                ConfigureProductionLogging(configuration, context.Configuration);
            }
        });

        return builder;
    }

    private static void ConfigureDevelopmentLogging(
        LoggerConfiguration configuration,
        IConfiguration appConfig)
    {
        configuration
            .MinimumLevel.Debug()
            .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
            .MinimumLevel.Override("Microsoft.AspNetCore", LogEventLevel.Warning)
            .MinimumLevel.Override("Microsoft.EntityFrameworkCore", LogEventLevel.Information);

        // The console and Seq are already configured via ReadFrom.Configuration
        // But we can override the console format
        configuration.WriteTo.Console(
            outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] [{ServiceName}] {Message:lj} {Properties:j}{NewLine}{Exception}");
    }

    private static void ConfigureProductionLogging(
        LoggerConfiguration configuration,
        IConfiguration appConfig)
    {
        configuration
            .MinimumLevel.Warning()
            .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
            .MinimumLevel.Override("Microsoft.AspNetCore", LogEventLevel.Error)
            .WriteTo.Console(
                outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss} [{Level:u3}] [{ServiceName}] {Message:lj}{NewLine}{Exception}")
            .WriteTo.File(
                path: "logs/smartevent-.log",
                rollingInterval: RollingInterval.Day,
                retainedFileCountLimit: 7,
                outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff} [{Level:u3}] [{ServiceName}] {Message:lj} {Properties:j}{NewLine}{Exception}");
    }

    public static WebApplication UseSmartEventLogging(this WebApplication app)
    {
        app.UseSerilogRequestLogging(options =>
        {
            options.MessageTemplate =
                "HTTP {RequestMethod} {RequestPath} responded {StatusCode} in {Elapsed:0.0000} ms";

            options.GetLevel = (httpContext, elapsed, ex) =>
            {
                if (ex != null || httpContext.Response.StatusCode >= 500)
                    return LogEventLevel.Error;

                if (httpContext.Response.StatusCode >= 400)
                    return LogEventLevel.Warning;

                return LogEventLevel.Information;
            };

            options.EnrichDiagnosticContext = (diagnosticContext, httpContext) =>
            {
                diagnosticContext.Set("RequestHost", httpContext.Request.Host.Value);
                diagnosticContext.Set("RequestScheme", httpContext.Request.Scheme);
                diagnosticContext.Set("RemoteIp", httpContext.Connection.RemoteIpAddress?.ToString());
            };
        });

        return app;
    }
}