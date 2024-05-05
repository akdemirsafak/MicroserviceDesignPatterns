using MassTransit;
using Microsoft.EntityFrameworkCore;
using SharedLib;
using Stock.API.DbContexts;

namespace Stock.API.Consumers
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
            foreach (var item in context.Message.OrderItems)
            {
                var stock= await _dbContext.Stocks.SingleOrDefaultAsync(x=>x.ProductId==item.ProductId);
                if (stock is null)
                {
                    _logger.LogError("Product not found.");
                }
                else
                {
                    stock.Count += item.Count;
                }
            }
            await _dbContext.SaveChangesAsync();
            _logger.LogInformation($"Stocks are released for Order Id : {context.Message.OrderId}");

        }
    }
}
