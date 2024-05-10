using System.Text;
using System.Text.Json;
using EventSourcing.API.DbContexts;
using EventSourcing.API.Entities;
using EventSourcing.API.EventStores;
using EventSourcing.Shared.Events;
using EventStore.ClientAPI;

namespace EventSourcing.API.BackgroundServices
{
    public class ProductReadModelEventStore : BackgroundService
    {

        private readonly IEventStoreConnection _eventStoreConnection;
        private readonly ILogger<ProductReadModelEventStore> _logger;
        private readonly IServiceProvider _serviceProvider;
        //BackgroundService'ler singleton'dır bu sebeple içerisinde Scoped service ekleyemeyiz. Bu sebeple Scoped service eklememiz için IServiceProvider aldık.


        public ProductReadModelEventStore(
            IEventStoreConnection eventStoreConnection,
            ILogger<ProductReadModelEventStore> logger,
            IServiceProvider serviceProvider)
        {
            _eventStoreConnection = eventStoreConnection;
            _logger = logger;
            _serviceProvider = serviceProvider;
        }

        public override Task StartAsync(CancellationToken cancellationToken)
        {
            //Uygulama başladığında
            return base.StartAsync(cancellationToken);
        }
        public override Task StopAsync(CancellationToken cancellationToken)
        {
            //Uygulama down olmadan önce çalışacaklar.
            return base.StopAsync(cancellationToken);
        }
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await _eventStoreConnection.ConnectToPersistentSubscriptionAsync(ProductStream.StreamName,
            ProductStream.GroupName,
            EventAppeared,
            autoAck: false); //Bu parametre true olarak set edilirse EventStore bir mesaj gönderildiğinde direkt olarak o mesajı gönderilmiş sayar; doğru olarak işlenmişmi durumunu önemsemez.(EventAppeared bir hata fırlatmazsa gönderilmiş sayar.Hata varsa bir sonraki adımda tekrar gönderir.)
            //False'a set ettiğimiz için işlem başarılıysa kendimiz eventin başarılı olduğunu döneceğiz.
            //Rabbitmq gibi message broker'larda mesaj subscribe olan service'de işlendikten sonra message broker'a mesajın başarıyla işlendiğine dair dönüş yapar ve kuyruktan silmesini söyler. 
        }
        private async Task EventAppeared(EventStorePersistentSubscriptionBase arg1, ResolvedEvent arg2)
        {
            var type = Type.GetType($"{Encoding.UTF8.GetString(arg2.Event.Metadata)}, EventSourcing.Shared"); //Almak istediğimi tipler ayrı class'larda olduğu için EventSourcing.Shared diyerek özellikle belirttik.Aynı namespace altında olsaydı belirtmemize gerek kalmayacaktı.
            _logger.LogInformation($"The message processing... : {type} ");
            var eventData = Encoding.UTF8.GetString(arg2.Event.Data);
            var @event = JsonSerializer.Deserialize(eventData, type); //event özel bir keyword olduğu için sıkıntı yaşatmamak açısından @ ile kullandık.

            //
            using var scope = _serviceProvider.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            //

            Product? product;
            switch (@event)
            {
                case ProductCreatedEvent productCreatedEvent:
                    product = new Product()
                    {
                        Id = productCreatedEvent.Id, //Burada id olmasının sebebi EventStore'a kaydederken id oluşturuluyordu ve bu id'nin ReadDb'de aynı olmalı.
                        Name = productCreatedEvent.Name,
                        Price = productCreatedEvent.Price,
                        Stock = productCreatedEvent.Stock,
                        UserId = productCreatedEvent.UserId
                    };
                    await dbContext.Products.AddAsync(product);
                    break;
                case ProductNameChangedEvent productNameChangedEvent:
                    product = await dbContext.Products.FindAsync(productNameChangedEvent.Id);
                    if (product != null)
                    {
                        product.Name = productNameChangedEvent.Name;
                    }
                    break;
                case ProductPriceChangedEvent productPriceChangedEvent:
                    product = await dbContext.Products.FindAsync(productPriceChangedEvent.Id);
                    if (product != null)
                    {
                        product.Price = productPriceChangedEvent.ChangedPrice;
                    }
                    break;
                case ProductDeletedEvent productDeletedEvent:
                    product = await dbContext.Products.FindAsync(productDeletedEvent.Id);
                    if (product != null)
                    {
                        dbContext.Products.Remove(product);
                    }
                    break;
            }
            await dbContext.SaveChangesAsync();

            arg1.Acknowledge(arg2.Event.EventId); //EventAppeared başarıyla çalıştı mesajını manuel olarak EventStore'a gönderdik.Bunun sebebi autoAck'yı false olarak ayarlamamız.!
        }
    }
}