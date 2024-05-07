using SharedLib.Interfaces;

namespace SharedLib.Events
{
    public class StockNotReservedEvent : IStockNotReservedEvent
    {
        public StockNotReservedEvent(Guid correllationId)
        {
            CorrelationId = correllationId;
        }
        public string Reason { get; set; }

        public Guid CorrelationId { get; }
    }
}