using OpenTelemetry.Resources;

namespace OpenMatchFrontend.Observability;

public static class OtelResourceBuilder
{
    public static ResourceBuilder ResourceBuilder { get; } = ResourceBuilder
        .CreateDefault()
        .AddService(GlobalConsts.ServiceName, null, GlobalConsts.ServiceVersion);
    
}