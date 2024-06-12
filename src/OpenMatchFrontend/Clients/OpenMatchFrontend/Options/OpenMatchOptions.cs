namespace OpenMatchFrontend.Clients.OpenMatchFrontend;

public sealed class OpenMatchOptions
{
    public const string SectionName = "OpenMatch";

    public const string OpenMatchFrontendDefaultAddress = "http://open-match-frontend.open-match.svc.cluster.local:50504";

    public string FrontendServiceAddress { get; init; } = OpenMatchFrontendDefaultAddress;
}