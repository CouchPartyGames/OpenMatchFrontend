using OpenTelemetry.Exporter;
using OpenTelemetry.Logs;
using OpenTelemetry.Resources;

namespace OpenMatchFrontend.Observability;

public static class LoggingInjection
{

    public static ILoggingBuilder AddObservabilityLogging(this ILoggingBuilder loggingBuilder,
        IConfiguration configuration,
        ResourceBuilder resourceBuilder)
    {
        var options = configuration
            .GetSection(OpenTelemetryOptions.SectionName)
            .Get<OpenTelemetryOptions>();
        
        loggingBuilder.ClearProviders();
        loggingBuilder.AddOpenTelemetry(opts =>
        {
            opts.SetResourceBuilder(resourceBuilder);
            opts.AddOtlpExporter(export =>
            {
                export.Endpoint = new Uri(OpenTelemetryOptions.OtelDefaultEndpoint);
                export.Protocol = OtlpExportProtocol.Grpc;
            });
        });
        return loggingBuilder;
    }
}