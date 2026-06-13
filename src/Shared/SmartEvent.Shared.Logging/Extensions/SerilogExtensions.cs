using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using Serilog.Events;

namespace SmartEvent.Shared.Logging.Extensions;

public static class SerilogExtensions
{
    public static WebApplicationBuilder AddCustomSerilog(this WebApplicationBuilder builder, string serviceName)
    {
        SerilogConfiguration.ConfigureLogging(serviceName);
        builder.Host.UseSerilog();
        return builder;
    }

    public static IServiceCollection AddRequestLogging(this IServiceCollection services)
    {
        services.AddHttpLogging(options =>
        {
            options.LoggingFields = Microsoft.AspNetCore.HttpLogging.HttpLoggingFields.All;
        });

        return services;
    }
}