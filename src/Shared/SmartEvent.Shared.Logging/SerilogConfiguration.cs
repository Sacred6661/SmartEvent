using Serilog;
using Serilog.Events;
using Microsoft.Extensions.Hosting;

namespace SmartEvent.Shared.Logging;

public static class SerilogConfiguration
{
    public static void ConfigureLogging(string serviceName)
    {
        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Debug()
            .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
            .MinimumLevel.Override("System", LogEventLevel.Warning)
            .Enrich.FromLogContext()
            .Enrich.WithMachineName()
            .Enrich.WithEnvironmentName()
            .Enrich.WithThreadId()
            .Enrich.WithProperty("ServiceName", serviceName)
            .WriteTo.Console(
                outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj} {Properties:j}{NewLine}{Exception}")
            .WriteTo.Seq(
                serverUrl: "http://localhost:5341",
                apiKey: null) // need api key for production
            .CreateLogger();
    }
}