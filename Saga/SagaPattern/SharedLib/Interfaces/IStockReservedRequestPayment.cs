using MassTransit;

namespace SharedLib.Interfaces
{
    public interface IStockReservedRequestPayment : CorrelatedBy<Guid>
    {
        public PaymentMessage Payment { get; set; }
        public string BuyerId { get; set; }
        public List<OrderItemMessage> OrderItems { get; set; } //Ödeme başarısız olursa stokları geri almak için.
    }
}
