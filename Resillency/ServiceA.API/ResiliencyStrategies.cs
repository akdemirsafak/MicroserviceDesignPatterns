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

    //Basit Circuit Breaker yapısı
    public static IAsyncPolicy<HttpResponseMessage> GetCircuitBreakerPolicy()
    {
        return HttpPolicyExtensions
            .HandleTransientHttpError() //OrResult kullanmadığımız için her hata durumunu kapsar.
            .CircuitBreakerAsync(3, TimeSpan.FromSeconds(10),//Burada belirlenen sürede 3 başarısız istek yapılması durumu söz konusudur.
            onBreak: (arg1, arg2) =>
            {//Delege ve timespan
                Console.WriteLine("-------------------------------------");
                Console.WriteLine("Circuit is open => On Break");
                Debug.WriteLine("Circuit is open=> On Break");

                Console.WriteLine("-------------------------------------");
            },
            onReset: () =>
            { // kapalı olma durumu 

                Console.WriteLine("-------------------------------------");
                Console.WriteLine("Circuit is closed => On Reset");
                Debug.WriteLine("Circuit is closed => On Reset");

                Console.WriteLine("-------------------------------------");
            },
            onHalfOpen: () =>
            {

                Console.WriteLine("-------------------------------------");
                Console.WriteLine("Circuit is half open => On Half Open");
                Debug.WriteLine("Circuit is half open => On Half Open");

                Console.WriteLine("-------------------------------------");
            });
    }


    //Advanced Circuit Breaker yapısı
    public static IAsyncPolicy<HttpResponseMessage> GetAdvancedCircuitBreakerPolicy()
    {
        return HttpPolicyExtensions
            .HandleTransientHttpError()
            .AdvancedCircuitBreakerAsync(0.5, TimeSpan.FromSeconds(10), 2, TimeSpan.FromSeconds(30), // Poly 0.5 tavsiye eder.
            //0.5 yüzdelik oranı ifade eder.10 saniye içerisinde gelen isteklerin %50 si (0.5) başarısızsa aktifleştirir.
            //3. parametre de minimum başarısız deneme sayısını ifade eder.Yani %20 si başarısız olmak şartıyla 10 istek yapılırsa 5 başarısız istekten az olduğu için devreye girmeyecek.
            //4. parametre ise devre dışı kalma/sıfırlanma süresini ifade eder.
            onBreak: (arg1, arg2) =>
            {
                Debug.WriteLine($"Circuit is open=> On Break :{arg2.TotalSeconds}");
                Console.WriteLine("Circuit is open=> On Break");
            },
            onReset: () =>
            {
                Debug.WriteLine("Circuit is closed => On Reset");
                Console.WriteLine("Circuit is closed => On Reset");
            },
            onHalfOpen: () =>
            {
                Debug.WriteLine("Circuit is half open => On Half Open");
                Console.WriteLine("Circuit is half open => On Half Open");
            });
    }
}