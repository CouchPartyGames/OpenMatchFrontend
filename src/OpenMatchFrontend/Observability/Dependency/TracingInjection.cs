﻿using OpenMatchFrontend.Observability.Options;
using OpenTelemetry.Exporter;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

namespace OpenMatchFrontend.Observability.Dependency;

public static class TracingInjection
{ 
    public static IServiceCollection AddObservabilityTraces(this IServiceCollection services,
        IConfiguration configuration,
        ResourceBuilder resourceBuilder)
    {
        var options = configuration
            .GetSection(OpenTelemetryOptions.SectionName)
            .Get<OpenTelemetryOptions>();
        
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
                    opts.Endpoint = new Uri(OpenTelemetryOptions.OtelDefaultHost);
                    opts.Protocol = OtlpExportProtocol.Grpc;
                });
            });
        return services;
    }
    
}