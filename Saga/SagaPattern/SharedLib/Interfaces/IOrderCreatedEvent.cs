using MassTransit;

namespace SharedLib.Interfaces
{
    public interface IOrderCreatedEvent : CorrelatedBy<Guid>
    {
        //Her eventte bir corellationId olmalı.
        public List<OrderItemMessage> OrderItems { get; set; } //Taşımak istediğimiz data
    }
}
