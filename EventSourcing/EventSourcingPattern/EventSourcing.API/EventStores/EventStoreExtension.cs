using EventStore.ClientAPI;

namespace EventSourcing.API.EventStores
{
    public static class EventStoreExtension
    {
        public static void AddEventStore(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            var eventStoreConnection = EventStoreConnection.Create(
                configuration.GetConnectionString("EventStoreConnection"));

            eventStoreConnection.ConnectAsync().Wait();

            services.AddSingleton(eventStoreConnection);

            using var logFactory = LoggerFactory.Create(builder =>
            {
                builder.SetMinimumLevel(LogLevel.Information);
                builder.AddConsole();
            });
            var logger = logFactory.CreateLogger(nameof(Program));

            eventStoreConnection.Connected += (sender, args) => logger.LogInformation("Connected to EventStore");

            eventStoreConnection.ErrorOccurred += (sender, args) => logger.LogError(args.Exception.Message);

            eventStoreConnection.Disconnected += (sender, args) => logger.LogInformation("Disconnected from EventStore");
        }
    }
}
