using Microsoft.Extensions.Options;
using Serilog;
using Serilog.Events;
using Serilog.Filters;

namespace GolbonWebRoad.Api;

public static class AddSerilogExtension
{
    private const string DefaultOutputTemplate =
        "[{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} {Level:u3}] {Message:lj} {Properties:j}{NewLine}{Exception}";

    public static void UseCustomSerilog(this WebApplicationBuilder builder, Action<SeqOptions>? configureOptions = null)
    {
        ArgumentNullException.ThrowIfNull(builder);

        builder.Logging.ClearProviders();
        builder.Host.UseSerilog();
        builder.Services.Configure<SeqOptions>(builder.Configuration.GetSection(SeqOptions.SectionName));
        builder.Services.AddSerilogLogging(builder.Configuration, builder.Environment, configureOptions);
    }

    /// <summary>
    /// Adds Serilog with Elasticsearch integration and dynamic sink selection to the service collection.
    /// </summary>
    private static void AddSerilogLogging(
        this IServiceCollection services,
        ConfigurationManager configuration,
        IHostEnvironment hostEnvironment,
        Action<SeqOptions>? configureOptions = null)
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentNullException.ThrowIfNull(configuration);
        ArgumentNullException.ThrowIfNull(hostEnvironment);

        var optionsBuilder = services.AddOptions<SeqOptions>()
            .Bind(configuration.GetSection(SeqOptions.SectionName))
            .ValidateDataAnnotations()
            .ValidateOnStart();

        if (configureOptions != null)
        {
            optionsBuilder.Configure(configureOptions);
        }

        // Configure Serilog
        services.AddSerilog((serviceProvider, loggerConfiguration) =>
        {
            ConfigureSerilog(loggerConfiguration, serviceProvider, hostEnvironment);
        });
    }

    private static void ConfigureSerilog(
        LoggerConfiguration loggerConfiguration,
        IServiceProvider serviceProvider,
        IHostEnvironment hostEnvironment)
    {
        try
        {
            var options = serviceProvider.GetRequiredService<IOptions<SeqOptions>>().Value;

            // Configure base logger settings
            ConfigureBaseLogger(loggerConfiguration, hostEnvironment, options);

            // Configure console sink for development
            if (hostEnvironment.IsDevelopment())
            {
                loggerConfiguration.WriteTo.Console(outputTemplate: DefaultOutputTemplate);
            }

            // Configure sinks based on strategy
            if (options.SeqEnabled)
            {
                loggerConfiguration.WriteTo.Seq(options.ServerUrl, options.MinimumLevel);
            }
        }
        catch (Exception ex)
        {
            // Fallback to console logging if configuration fails
            Console.WriteLine($"[ERROR] Failed to configure Serilog sinks. Falling back to console logging: {ex.Message}");

            loggerConfiguration
                .MinimumLevel.Information()
                .WriteTo.Console(outputTemplate: DefaultOutputTemplate);
        }
    }


    private static void ConfigureBaseLogger(
        LoggerConfiguration loggerConfiguration,
        IHostEnvironment hostEnvironment,
        SeqOptions options)
    {
        loggerConfiguration
            .MinimumLevel.Is(options.MinimumLevel)
            .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
            .MinimumLevel.Override("Microsoft.Hosting.Lifetime", LogEventLevel.Information)
            .MinimumLevel.Override("Microsoft.EntityFrameworkCore", LogEventLevel.Warning)
            .MinimumLevel.Override("System", LogEventLevel.Warning)
            .MinimumLevel.Override("Microsoft.AspNetCore.Authentication", LogEventLevel.Information)
            .Enrich.FromLogContext()
            .Enrich.WithProcessId()
            .Enrich.WithProcessName()
            .Enrich.WithMachineName()
            .Enrich.WithProperty("Application", "GolbonWebRoad")
            .Enrich.WithProperty("Environment", hostEnvironment.EnvironmentName)
            .Filter.ByExcluding(Matching.FromSource("Microsoft.AspNetCore.StaticFiles"))
            .Filter.ByExcluding(Matching.FromSource("Microsoft.AspNetCore.Routing.EndpointMiddleware"))
            .Filter.ByExcluding(Matching.FromSource("Microsoft.AspNetCore.Hosting.Diagnostics"));
    }
}

public class SeqOptions
{
    public const string SectionName = "Seq";

    public string ServerUrl { get; set; }
    public LogEventLevel MinimumLevel { get; set; }
    public bool SeqEnabled { get; set; }
}