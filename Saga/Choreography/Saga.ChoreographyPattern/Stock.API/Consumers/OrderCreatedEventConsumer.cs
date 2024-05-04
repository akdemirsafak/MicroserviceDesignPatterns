using MassTransit;
using Microsoft.EntityFrameworkCore;
using SharedLib;
using Stock.API.DbContexts;

namespace Stock.API.Consumers
{
    public class OrderCreatedEventConsumer : IConsumer<OrderCreatedEvent> //OrderCreatedEvent'i dinleyecek ve bu event fırlatıldığında bu consumer yakalayacak.
    {
        private readonly AppDbContext _dbContext;
        private readonly ILogger<OrderCreatedEventConsumer> _logger;
        private readonly ISendEndpointProvider _sendEndpointProvider;
        private readonly IPublishEndpoint _publishEndpoint;

        public OrderCreatedEventConsumer(AppDbContext dbContext,
        ILogger<OrderCreatedEventConsumer> logger,
        ISendEndpointProvider sendEndpointProvider,
        IPublishEndpoint publishEndpoint)
        {
            _dbContext = dbContext;
            _logger = logger;
            _sendEndpointProvider = sendEndpointProvider;
            _publishEndpoint = publishEndpoint;
        }

        public async Task Consume(ConsumeContext<OrderCreatedEvent> context)
        {
            //Eventimizi dinleyen tek servisimiz olduğu için Send methodunu kullanacağız ve queue adını vereceğiz.

            //Tüm ürünler stokta varsa sipariş verilebilecek senaryosu üzerinden ilerliyoruz.

            var stockResult = new List<bool>();
            foreach (var item in context.Message.OrderItems)
            {
                stockResult.Add(await _dbContext.Stocks.AnyAsync(x => x.ProductId == item.ProductId && x.Count > item.Count));
            }
            if (stockResult.All(x => x.Equals(true)))
            {

                //Stoklar yeterli ise stokları düşüreceğiz.
                foreach (var item in context.Message.OrderItems)
                {
                    var stock = await _dbContext.Stocks.FirstOrDefaultAsync(x => x.ProductId == item.ProductId);
                    if (stock is not null)
                        stock.Count -= item.Count;

                    _dbContext.Stocks.Update(stock);
                }
                await _dbContext.SaveChangesAsync();
                _logger.LogInformation($"Stock was reserved for BuyerId : {context.Message.BuyerId}");
                var sendEndpoint = await _sendEndpointProvider.GetSendEndpoint(new Uri($"queue:{RabbitMQSettingsConst.StockReservedEventQueueName}"));

                #region 
                var stockReservedEvent = new StockReservedEvent()
                {
                    OrderId = context.Message.OrderId,
                    BuyerId = context.Message.BuyerId,
                    Payment = context.Message.Payment,
                    OrderItems = context.Message.OrderItems
                };
                #endregion

                await sendEndpoint.Send(stockReservedEvent);

            }
            await _publishEndpoint.Publish(new StockNotReservedEvent()
            {
                OrderId = context.Message.OrderId,
                Message = "Stock is not enough"
            });
            _logger.LogInformation($"Stock wasnt reserved.Stock is enough.");
        }
    }
}