using GameFrontend.Endpoints;
using Microsoft.AspNetCore.HttpLogging;
using OpenMatchFrontend.Exceptions;
using OpenTelemetry.Exporter;
using OpenTelemetry.Logs;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

var builder = WebApplication.CreateSlimBuilder(args);   // .NET 8 + AOT

var resourceBuilder = ResourceBuilder
    .CreateDefault()
    .AddService("OpenFrontend", null, "1.0.0");

builder.Logging.ClearProviders();
builder.Logging.AddOpenTelemetry(opts =>
{
    opts.SetResourceBuilder(resourceBuilder);
    opts.AddOtlpExporter(export =>
    {
        export.Endpoint = new Uri("http://localhost:4317");
        export.Protocol = OtlpExportProtocol.Grpc;
    });
});
builder.Services.AddExceptionHandler<DefaultExceptionHandler>();
builder.Services.AddHttpContextAccessor();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddHealthChecks();
builder.Services.AddHttpLogging(o =>
{
    o.LoggingFields = HttpLoggingFields.All;
    o.CombineLogs = true;
});
builder.Services.AddGrpcClient<FrontendService.FrontendServiceClient>(o =>
{
    var address = builder.Configuration["OPENMATCH_FRONTEND_HOST"] ??
                  "http://open-match-frontend.open-match.svc.cluster.local:50504";
    o.Address = new Uri(address);
}).ConfigureChannel(o =>
{
    o.HttpHandler = new SocketsHttpHandler()
    {
        KeepAlivePingDelay = TimeSpan.FromSeconds(60),
        KeepAlivePingTimeout = TimeSpan.FromSeconds(30),
        EnableMultipleHttp2Connections = true
    };
    o.MaxRetryAttempts = 4;
}).AddStandardResilienceHandler();

builder.Services.AddOpenTelemetry()
    .WithTracing(traceBuilder =>
    {
        traceBuilder.SetResourceBuilder(resourceBuilder);
        traceBuilder.SetSampler(new TraceIdRatioBasedSampler(1.0));
        traceBuilder.AddHttpClientInstrumentation()
            .AddAspNetCoreInstrumentation()
            .AddGrpcClientInstrumentation();

        traceBuilder.AddOtlpExporter(opts =>
        {
            opts.Endpoint = new Uri("http://localhost:4317");
            opts.Protocol = OtlpExportProtocol.Grpc;
        });
    })
    .WithMetrics(metricBuilder =>
    {
        metricBuilder.SetResourceBuilder(resourceBuilder);
        metricBuilder
            .AddRuntimeInstrumentation()
            .AddHttpClientInstrumentation()
            .AddAspNetCoreInstrumentation();
        metricBuilder.AddOtlpExporter(export =>
        {
            export.Endpoint = new Uri("http://localhost:4317");
            export.Protocol = OtlpExportProtocol.Grpc;
        });
    });

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Graceful Termination for Kubernetes
CancellationTokenSource cancellation = new();
app.Lifetime.ApplicationStopping.Register(() =>
{
    cancellation.Cancel();
});

app.UseExceptionHandler(options => { });
app.UseHttpLogging();
app.MapHealthChecks("/health");
app.MapAuthenticationEndpoints();
app.MapTicketEndpoints();

app.Run();
