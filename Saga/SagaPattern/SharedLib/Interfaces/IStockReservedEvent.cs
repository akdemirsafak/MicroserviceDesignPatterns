using MassTransit;

namespace SharedLib.Interfaces
{
    public interface IStockReservedEvent:CorrelatedBy<Guid>
    {
        public List<OrderItemMessage> OrderItems { get; set; }
    }
}
