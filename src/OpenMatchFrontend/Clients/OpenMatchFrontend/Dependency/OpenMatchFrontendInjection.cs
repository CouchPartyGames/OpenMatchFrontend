using OpenMatchFrontend.Interceptors;

namespace OpenMatchFrontend.Clients.OpenMatchFrontend.Dependency;

public static class OpenMatchFrontendInjection
{

    public static IServiceCollection AddOpenMatchFrontendClient(this IServiceCollection services,
        IConfiguration configuration)
    { 
        services.Configure<OpenMatchOptions>(
            configuration.GetSection(OpenMatchOptions.SectionName));
        
        services
            .AddGrpcClient<FrontendService.FrontendServiceClient>(o =>
            {
                var frontend = configuration.Get<OpenMatchOptions>().FrontendServiceAddress;
                var address = frontend ??
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
        
        services.AddSingleton<ClientLoggerInterceptor>();
        
        return services;
    }
}