using MassTransit;
using Microsoft.EntityFrameworkCore;
using SharedLib.Messages;
using Stock.API.DbContexts;

namespace Stock.API.Consumers
{
    public class StockRollBackMessageConsumer : IConsumer<IStockRollBackMessage>
    {
        private readonly AppDbContext _dbContext;
        private readonly ILogger<StockRollBackMessageConsumer> _logger;

        public StockRollBackMessageConsumer(
            AppDbContext dbContext, 
            ILogger<StockRollBackMessageConsumer> logger)
        {
            _dbContext = dbContext;
            _logger = logger;
        }

        public async Task Consume(ConsumeContext<IStockRollBackMessage> context)
        {
            foreach (var orderItem in context.Message.OrderItems)
            {
                var stock =await _dbContext.Stocks.FirstOrDefaultAsync(x=>x.ProductId==orderItem.ProductId);
                if (stock == null)
                {
                    _logger.LogError($"Product {orderItem.ProductId} not found.");
                }
                else
                {
                    stock.Count += orderItem.Count;
                    _logger.LogInformation($"Stock {orderItem.ProductId} rollback  {orderItem.Count} Total : {stock.Count}");
                }
            }
            await _dbContext.SaveChangesAsync();

        }
    }
}
