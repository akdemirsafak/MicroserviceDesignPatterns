using SharedLib.Interfaces;

namespace SharedLib.Events
{
    public class PaymentCompletedEvent : IPaymentCompletedEvent
    {
        public PaymentCompletedEvent(Guid correlationId)
        {
            CorrelationId = correlationId;
        }
        public Guid CorrelationId { get; }
    }
}
