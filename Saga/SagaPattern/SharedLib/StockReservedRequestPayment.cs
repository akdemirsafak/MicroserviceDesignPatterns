using SharedLib.Interfaces;

namespace SharedLib
{
    public class StockReservedRequestPayment : IStockReservedRequestPayment
    {
        public StockReservedRequestPayment(Guid correllationId)
        {
            CorrelationId = correllationId;
        }
        public PaymentMessage Payment { get; set; }
        public List<OrderItemMessage> OrderItems { get; set; }
        public string BuyerId { get; set; }

        public Guid CorrelationId { get; }
    }
}
