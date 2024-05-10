using MassTransit;

namespace SharedLib.Interfaces
{
    public interface IPaymentFailedEvent : CorrelatedBy<Guid>
    {

        public string Reason { get; set; }
        public List<OrderItemMessage> OrderItems { get; set; } //Db'de tutsaydık burada taşımamıza gerek kalmayacaktı.
    }
}