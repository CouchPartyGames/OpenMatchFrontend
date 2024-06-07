namespace OpenMatchFrontend.Clients.OpenMatchFrontend;

public sealed class OpenMatchOptions
{
    public const string SectionName = "OpenMatch";

    public string FrontendServiceAddr { get; init; } = "http://open-match-frontend.open-match.svc.cluster.local:50504";
}