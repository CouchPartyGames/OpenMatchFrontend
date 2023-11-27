using GameFrontend.Endpoints;
using GameFrontend.OpenMatch;
using Microsoft.AspNetCore.HttpLogging;
using Microsoft.Extensions.Http.Resilience;
using OpenMatchFrontend.Exceptions;
using OpenTelemetry.Metrics;
using Serilog;
using Serilog.Formatting.Compact;

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console(new CompactJsonFormatter())
    .CreateLogger();

var builder = WebApplication.CreateSlimBuilder(args);   // .NET 8 + AOT

//builder.Logging.ClearProviders();
//builder.Logging.AddConsole();

builder.Host.UseSerilog();
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
    o.MaxRetryAttempts = 4;
}).AddStandardResilienceHandler();

builder.Services.AddOpenTelemetry()
    .WithMetrics(x =>
    {
        x.AddMeter("Microsoft.AspNetCore.Hosting", 
            "Microsoft.AspNetCore.Server.Kestrel",
            "System.Net.Http");
        x.AddPrometheusExporter();
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

app.UseHttpLogging();
app.MapHealthChecks("/health");
app.MapPrometheusScrapingEndpoint();
app.MapAuthenticationEndpoints();
app.MapTicketEndpoints();

app.Run();
