using SharedLib.Interfaces;

namespace SharedLib
{
    public class OrderCreatedEvent : IOrderCreatedEvent
    {
        public OrderCreatedEvent(Guid correlationId)
        {
            CorrelationId = correlationId;
        }
        //Buradan OrderId, BuyerId, Payment gibi bilgileri kaldırmamızın sebebi bu bilgilerin Initial aşamasında requestten alınıp State Machine'de tutuluyor olması.
        public List<OrderItemMessage> OrderItems { get; set; } = new();

        public Guid CorrelationId { get; }
      
    }
}