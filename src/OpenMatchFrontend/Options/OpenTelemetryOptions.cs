namespace OpenMatchFrontend.Options;

public sealed class OpenTelemetryOptions
{
   public const string SectionName = "OpenTelemetry";

   public string Endpoint { get; init; } = "http://localhost:4317";
}