using MassTransit;
using Order.API.Models;
using SharedLib.Events;

namespace Order.API.Consumers
{
    public class OrderRequestFailedEventConsumer : IConsumer<OrderRequestFailedEvent>
    {
        private readonly AppDbContext _dbContext;
        private readonly ILogger<OrderRequestCompletedEventConsumer> _logger;

        public OrderRequestFailedEventConsumer(
            AppDbContext dbContext,
            ILogger<OrderRequestCompletedEventConsumer> logger)
        {
            _dbContext = dbContext;
            _logger = logger;
        }

        public async Task Consume(ConsumeContext<OrderRequestFailedEvent> context)
        {
            var order = await _dbContext.Orders.FindAsync(context.Message.OrderId);
            if (order == null)
            {
                _logger.LogError($"Order {context.Message.OrderId} not found");
            }
            else
            {
                order.Status = OrderStatus.Failed;
                order.FailMessage = context.Message.Reason;
                await _dbContext.SaveChangesAsync();
                _logger.LogInformation($"Order {context.Message.OrderId} status updated(Failed)");
            }
        }
    }
}
