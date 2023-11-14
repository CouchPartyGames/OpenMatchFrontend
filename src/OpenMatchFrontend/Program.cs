using GameFrontend.Endpoints;
using GameFrontend.OpenMatch;
using Microsoft.Extensions.Http.Resilience;
using OpenTelemetry.Metrics;
using Serilog;
using Serilog.Formatting.Compact;

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console(new CompactJsonFormatter())
    .CreateLogger();

var builder = WebApplication.CreateSlimBuilder(args);   // .NET 8 + AOT

builder.Host.UseSerilog();
builder.Services.AddHttpContextAccessor();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddHealthChecks();
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
        x.AddPrometheusExporter();
        x.AddMeter("Microsoft.AspNetCore.Hosting", "Microsoft.AspNetCore.Server.Kestrel");
    });

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

CancellationTokenSource cancellation = new();
app.Lifetime.ApplicationStopping.Register(() =>
{
    cancellation.Cancel();
});

app.UseSerilogRequestLogging();
app.MapHealthChecks("/health");
app.MapPrometheusScrapingEndpoint();
app.MapAuthenticationEndpoints();
app.MapTicketEndpoints();

app.Run();