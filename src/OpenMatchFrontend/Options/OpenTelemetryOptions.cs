namespace OpenMatchFrontend.Options;

public sealed class OpenTelemetryOptions
{
   public const string SectionName = "OpenTelemetry";

   public string Endpoint { get; init; } = "http://localhost:4317";

   public string Protocol { get; init; } = "grpc";

   public float SampleRate { get; init; } = 1.0f;
}