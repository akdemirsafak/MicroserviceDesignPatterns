using MassTransit;
using Order.API.Models;
using SharedLib;

namespace Order.API.Consumers
{
    public class StockNotReservedEventConsumer : IConsumer<StockNotReservedEvent>
    {
        private readonly AppDbContext _dbContext;
        private readonly ILogger<StockNotReservedEventConsumer> _logger;
        public StockNotReservedEventConsumer(AppDbContext dbContext, 
            ILogger<StockNotReservedEventConsumer> logger)
        {
            _dbContext = dbContext;
            _logger = logger;
        }

        public async Task Consume(ConsumeContext<StockNotReservedEvent> context)
        {
            var order= await _dbContext.Orders.FindAsync(context.Message.OrderId);
            if(order is not null)
            {
                order.Status = OrderStatus.Failed;
                order.FailMessage = context.Message.Message;
                await _dbContext.SaveChangesAsync();
                _logger.LogInformation($"Order {order.Id} failed : {context.Message.Message}");
            }
            else
            {
                _logger.LogError($"Order {context.Message.OrderId} not found");
            }
        }
    }
}
