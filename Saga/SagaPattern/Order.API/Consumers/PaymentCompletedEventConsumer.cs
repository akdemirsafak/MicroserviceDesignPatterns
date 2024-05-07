using MassTransit;
using Order.API.Models;
using SharedLib;

namespace Order.API.Consumers
{
    public class PaymentCompletedEventConsumer : IConsumer<PaymentCompletedEvent>
    {
        private readonly AppDbContext _dbContext;
        private readonly ILogger<PaymentCompletedEventConsumer> _logger;

        public PaymentCompletedEventConsumer(AppDbContext dbContext, 
            ILogger<PaymentCompletedEventConsumer> logger)
        {
            _dbContext = dbContext;
            _logger = logger;
        }

        public async Task Consume(ConsumeContext<PaymentCompletedEvent> context)
        {
            var order =await _dbContext.Orders.FindAsync(context.Message.OrderId);
            if (order is not null)
            {
                order.Status = OrderStatus.Completed;
                await _dbContext.SaveChangesAsync();
                _logger.LogInformation($"Order ({order.Id})'s status is updated to {order.Status}.Order Completed.");
            }
            else
            { 
                _logger.LogError($"Order ({context.Message.OrderId}) is not found.");
            }
        }
    }
}
