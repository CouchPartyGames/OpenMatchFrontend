namespace OpenMatchFrontend.Clients.OpenMatchFrontend;

public static class OpenMatchFrontendInjection
{

    public static IServiceCollection AddOpenMatchFrontendClient(this IServiceCollection services,
        IConfiguration configuration)
    {
        var options = configuration
            .GetSection(OpenMatchOptions.SectionName)
            .Get<OpenMatchOptions>();
        
        services.AddGrpcClient<FrontendService.FrontendServiceClient>(o =>
        {
            var address = configuration["OPENMATCH_FRONTEND_HOST"] ?? 
                          OpenMatchOptions.OpenMatchFrontendAddr;
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
        return services;
    }
}