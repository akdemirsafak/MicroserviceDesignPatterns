using MassTransit;
using Microsoft.EntityFrameworkCore;
using SharedLib.Events;
using SharedLib.Interfaces;
using Stock.API.DbContexts;

namespace Stock.API.Consumers
{
    public class OrderCreatedEventConsumer : IConsumer<IOrderCreatedEvent>
    {

        private readonly AppDbContext _dbContext;
        private readonly ILogger<OrderCreatedEventConsumer> _logger;
        private readonly IPublishEndpoint _publishEndpoint;

        public OrderCreatedEventConsumer(AppDbContext dbContext, 
            ILogger<OrderCreatedEventConsumer> logger, 

            IPublishEndpoint publishEndpoint)
        {
            _dbContext = dbContext;
            _logger = logger;
            _publishEndpoint = publishEndpoint;
        }

        public async Task Consume(ConsumeContext<IOrderCreatedEvent> context)
        {
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
                _logger.LogInformation($"Stock was reserved for CorellationId : {context.Message.CorrelationId}");

                var stockReservedEvent = new StockReservedEvent(context.Message.CorrelationId)
                {
                    OrderItems = context.Message.OrderItems
                };

                await _publishEndpoint.Publish(stockReservedEvent);
            }
            else
            {
                await _publishEndpoint.Publish(new StockNotReservedEvent(context.Message.CorrelationId)
                {
                    Reason = "Stock is not enough"
                });
                _logger.LogInformation($"Stock wasnt reserved.Stock is enough.");
            }
        }
    }
}

