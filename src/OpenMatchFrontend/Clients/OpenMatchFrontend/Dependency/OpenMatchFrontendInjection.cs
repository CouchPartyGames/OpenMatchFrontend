using OpenMatchFrontend.Interceptors;

namespace OpenMatchFrontend.Clients.OpenMatchFrontend.Dependency;

public static class OpenMatchFrontendInjection
{

    public static IServiceCollection AddOpenMatchFrontendClient(this IServiceCollection services,
        IConfiguration configuration)
    {
        var options = configuration
            .GetSection(OpenMatchOptions.SectionName)
            .Get<OpenMatchOptions>();
        
        services
            .AddGrpcClient<FrontendService.FrontendServiceClient>(o =>
            {
                var address = configuration["OPENMATCH_FRONTEND_HOST"] ??
                              OpenMatchOptions.OpenMatchFrontendDefaultAddress;
                o.Address = new Uri(address);
            })
            .ConfigureChannel(o =>
            {
                o.HttpHandler = new SocketsHttpHandler()
                {
                    KeepAlivePingDelay = TimeSpan.FromSeconds(60),
                    KeepAlivePingTimeout = TimeSpan.FromSeconds(30),
                    EnableMultipleHttp2Connections = true
                };
                o.MaxRetryAttempts = 4;
            })
            .AddInterceptor<ClientLoggerInterceptor>(InterceptorScope.Client)
            .AddStandardResilienceHandler();
        
        services.AddTransient<ClientLoggerInterceptor>();
        
        return services;
    }
}