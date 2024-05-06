using SharedLib.Interfaces;

namespace SharedLib
{
    public class StockReservedEvent : IStockReservedEvent
    {
        public StockReservedEvent(Guid correllationId)
        {
            CorrelationId = correllationId;
        }
        public List<OrderItemMessage> OrderItems { get ; set ; }

        public Guid CorrelationId { get; }
    }
}