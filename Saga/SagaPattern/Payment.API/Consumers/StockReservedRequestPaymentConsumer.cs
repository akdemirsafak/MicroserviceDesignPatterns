using MassTransit;
using SharedLib;
using SharedLib.Interfaces;

namespace Payment.API.Consumers
{
    public class StockReservedRequestPaymentConsumer : IConsumer<IStockReservedRequestPayment>
    {
        private readonly ILogger<StockReservedRequestPaymentConsumer> _logger;
        private readonly IPublishEndpoint _publishEndpoint;

        public StockReservedRequestPaymentConsumer(
            ILogger<StockReservedRequestPaymentConsumer> logger,
            IPublishEndpoint publishEndpoint)
        {
            _logger = logger;
            _publishEndpoint = publishEndpoint;
        }

        public async Task Consume(ConsumeContext<IStockReservedRequestPayment> context)
        {
            var balance = 3000m;
            if (balance > context.Message.Payment.TotalPrice)
            {
                _logger.LogInformation($"{context.Message.Payment.TotalPrice} TL was withdrawn from credit card. For user Id : {context.Message.BuyerId}");
                await _publishEndpoint.Publish(new PaymentCompletedEvent(context.Message.CorrelationId));
            }
            else
            {
                _logger.LogInformation($"Payment failed.Insufficient balance {balance-context.Message.Payment.TotalPrice} ");
                await _publishEndpoint.Publish(new PaymentFailedEvent(context.Message.CorrelationId)
                {
                    Reason = "Insufficient balance.",
                    OrderItems = context.Message.OrderItems
                });
            }
        }
    }
}