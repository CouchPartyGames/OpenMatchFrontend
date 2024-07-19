namespace OpenMatchFrontend.Observability.Options;

public sealed class OpenTelemetryOptions
{
   public const string SectionName = "OpenTelemetry";

   public const string OtelDefaultHost = "http://localhost:4317";

   public bool Enabled { get; init; } = false;

   public string Endpoint { get; init; } = OtelDefaultHost;

   public string Protocol { get; init; } = "grpc";

   public float SampleRate { get; init; } = 1.0f;
}