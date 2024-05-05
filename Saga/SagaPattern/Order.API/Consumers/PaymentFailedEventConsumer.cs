using MassTransit;
using Order.API.Models;
using SharedLib;

namespace Order.API.Consumers
{
    public class PaymentFailedEventConsumer : IConsumer<PaymentFailedEvent>
    {

        private readonly AppDbContext _dbContext;
        private readonly ILogger<PaymentFailedEventConsumer> _logger;

        public PaymentFailedEventConsumer(AppDbContext dbContext, 
            ILogger<PaymentFailedEventConsumer> logger)
        {
            _dbContext = dbContext;
            _logger = logger;
        }

        public async Task Consume(ConsumeContext<PaymentFailedEvent> context)
        {
            var order= await _dbContext.Orders.FindAsync(context.Message.OrderId);
            if (order is not null)
            {
                order.Status = OrderStatus.Failed;
                order.FailMessage= context.Message.Message;
                await _dbContext.SaveChangesAsync();
                _logger.LogInformation($"Order ({order.Id}) Failed.");
            }
            else
            {
                _logger.LogError($"Order ({context.Message.OrderId}) is not found.");
            }
        }
    }
}
