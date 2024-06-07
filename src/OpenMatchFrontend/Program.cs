using Microsoft.AspNetCore.HttpLogging;
using OpenMatchFrontend;
using OpenMatchFrontend.Clients.OpenMatchFrontend;
using OpenMatchFrontend.Endpoints;
using OpenMatchFrontend.Exceptions;
using OpenMatchFrontend.Observability;

var builder = WebApplication.CreateBuilder(args);   // .NET 8 
//var builder = WebApplication.CreateSlimBuilder(args);   // .NET 8 + AOT

    // Observability
builder.Logging.AddObservabilityLogging(builder.Configuration, OtelResourceBuilder.ResourceBuilder);
builder.Services.AddObservabilityMetrics(builder.Configuration, OtelResourceBuilder.ResourceBuilder);
builder.Services.AddObservabilityTraces(builder.Configuration, OtelResourceBuilder.ResourceBuilder);
builder.Services.AddHttpLogging(o =>
{
    o.LoggingFields = HttpLoggingFields.All;
    o.CombineLogs = true;
});

    // Clients
builder.Services.AddOpenMatchFrontendClient(builder.Configuration);

    // Service
builder.Services.AddExceptionHandler<DefaultExceptionHandler>();
builder.Services.AddHttpContextAccessor();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddHealthChecks();

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
app.MapHealthChecks(GlobalConsts.HealthPageUri);

app.MapAuthenticationEndpoints();
app.MapTicketEndpoints();

app.Run();
