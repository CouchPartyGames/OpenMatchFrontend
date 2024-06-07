using OpenTelemetry.Exporter;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

namespace OpenMatchFrontend.Observability;

public static class TraceInjection
{ 
    public static IServiceCollection AddObservabilityTraces(this IServiceCollection services,
        IConfiguration configuration,
        ResourceBuilder resourceBuilder)
    {
        services.AddOpenTelemetry()
            .WithTracing(traceBuilder =>
            {
                traceBuilder.SetResourceBuilder(resourceBuilder);
                traceBuilder.SetSampler(new TraceIdRatioBasedSampler(1.0));
                traceBuilder.AddHttpClientInstrumentation()
                    .AddAspNetCoreInstrumentation()
                    .AddGrpcClientInstrumentation();

                traceBuilder.AddOtlpExporter(opts =>
                {
                    opts.Endpoint = new Uri(OpenTelemetryOptions.OtelDefaultEndpoint);
                    opts.Protocol = OtlpExportProtocol.Grpc;
                });
            });
        return services;
    }
    
}