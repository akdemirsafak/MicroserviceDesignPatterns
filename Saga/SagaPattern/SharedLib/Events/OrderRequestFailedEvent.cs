using SharedLib.Interfaces;

namespace SharedLib.Events
{
    public class OrderRequestFailedEvent : IOrderRequestFailedEvent
    {
        public int OrderId { get ; set ; }
        public string Reason { get ; set ; }
    }
}
