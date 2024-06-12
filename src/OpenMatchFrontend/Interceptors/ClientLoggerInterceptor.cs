using Grpc.Core.Interceptors;

namespace OpenMatchFrontend.Interceptors;

public sealed class ClientLoggerInterceptor(ILogger<ClientLoggerInterceptor> logger) : Interceptor
{
    public override TResponse BlockingUnaryCall<TRequest, TResponse>(
        TRequest request,
        ClientInterceptorContext<TRequest, TResponse> context,
        BlockingUnaryCallContinuation<TRequest, TResponse> continuation)
    {
        LogCall(context.Method);

        try
        {
            return continuation(request, context);
        }
        catch (Exception ex)
        {
            LogError(ex);
            throw;
        }
    }
    
    public override AsyncUnaryCall<TResponse> AsyncUnaryCall<TRequest, TResponse>(
        TRequest request,
        ClientInterceptorContext<TRequest, TResponse> context,
        AsyncUnaryCallContinuation<TRequest, TResponse> continuation)
    {
        LogCall(context.Method);
        var call = continuation(request, context);

        return new AsyncUnaryCall<TResponse>(
            HandleResponse(call.ResponseAsync),
            call.ResponseHeadersAsync,
            call.GetStatus,
            call.GetTrailers,
            call.Dispose);
    }
    
    private async Task<TResponse> HandleResponse<TResponse>(Task<TResponse> inner)
    {
        try
        {
            return await inner;
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException("Custom error", ex);
        }
    }

    
    private void LogCall<TRequest, TResponse>(Method<TRequest, TResponse> method)
        where TRequest : class
        where TResponse : class
    {
        logger.LogInformation($"Starting call. Name: {method.Name}. Type: {method.Type}. Request: {typeof(TRequest)}. Response: {typeof(TResponse)}");
    }
    
    private void LogError(Exception ex)
    {
        logger.LogError(ex, $"Call error: {ex.Message}");
    }
}