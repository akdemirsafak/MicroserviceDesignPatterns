using MassTransit;
using SharedLib;

namespace Payment.API.Consumers
{
    public class StockReservedEventConsumer : IConsumer<StockReservedEvent>
    {
        private readonly ILogger<StockReservedEventConsumer> _logger;
        private readonly IPublishEndpoint _publishEndpoint;

        public StockReservedEventConsumer(ILogger<StockReservedEventConsumer> logger, 
            IPublishEndpoint publishEndpoint)
        {
            _logger = logger;
            _publishEndpoint = publishEndpoint;
        }

        public async Task Consume(ConsumeContext<StockReservedEvent> context)
        {
            decimal walletBalance = 5000m;
            if (walletBalance > context.Message.Payment.TotalPrice)
            {
                _logger.LogInformation($"{context.Message.Payment.TotalPrice} TL was withdrawn from credit card for user id= {context.Message.BuyerId}");
                //Ödeme işlemleri başarılı olduğunda PaymentSucceededEvent'i publish ediyoruz.
                await _publishEndpoint.Publish<PaymentCompletedEvent>(new
                {
                    context.Message.OrderId,
                    context.Message.BuyerId
                });
               
            }
            else
            {
                _logger.LogInformation($"Payment failed for order {context.Message.OrderId}");
                await _publishEndpoint.Publish<PaymentFailedEvent>(new
                {
                    context.Message.OrderId,
                    context.Message.BuyerId,
                    Message = "Insufficient balance"
                });
            }
        }
    }
}
