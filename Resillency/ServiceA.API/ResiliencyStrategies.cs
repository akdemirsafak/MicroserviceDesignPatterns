using Polly;
using Polly.Extensions.Http;
using System.Diagnostics;
using System.Net;

namespace ServiceA.API;

public static class ResiliencyStrategies
{
    public static IAsyncPolicy<HttpResponseMessage> GetRetryPolicy()
    {
        return HttpPolicyExtensions
            .HandleTransientHttpError()
            .OrResult(msg => msg.StatusCode == HttpStatusCode.NotFound)
            .WaitAndRetryAsync(6, retryAttempt =>
            {
                Debug.WriteLine($"Retry Count : {retryAttempt}");
                return TimeSpan.FromSeconds(3);
            }, onRetryAsync: onRetryAsync);
    }

    private static Task onRetryAsync(DelegateResult<HttpResponseMessage> arg1, TimeSpan arg2)
    {
        Console.WriteLine($" Request is made again {arg2.TotalMilliseconds}");
        Debug.WriteLine($" Request is made again {arg2.TotalMilliseconds}");
        return Task.CompletedTask;
    }
}