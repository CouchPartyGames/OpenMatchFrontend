using OpenMatchFrontend.Options;
using OpenTelemetry.Exporter;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;

namespace OpenMatchFrontend.Observability;

public static class MetricsInjection
{
    public static IServiceCollection AddObservabilityMetrics(this IServiceCollection services,
        IConfiguration configuration,
        ResourceBuilder resourceBuilder)
    {
        services.AddOpenTelemetry()
            .WithMetrics(metricBuilder =>
            {
                metricBuilder.SetResourceBuilder(resourceBuilder);
                metricBuilder
                    .AddRuntimeInstrumentation()
                    .AddHttpClientInstrumentation()
                    .AddAspNetCoreInstrumentation();
                metricBuilder.AddOtlpExporter(export =>
                {
                    export.Endpoint = new Uri(OpenTelemetryOptions.OtelDefaultEndpoint);
                    export.Protocol = OtlpExportProtocol.Grpc;
                });
            });
        
        return services;
    }
    
}