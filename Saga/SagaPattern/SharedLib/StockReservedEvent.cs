namespace SharedLib
{
    public class StockReservedEvent
    {//Bu eventi payment dinleyeceği için burada payment bilgileri geçilecek.

        public int OrderId { get; set; }
        public string BuyerId { get; set; }
        public PaymentMessage Payment { get; set; }
        public List<OrderItemMessage> OrderItems { get; set; } = new(); //Order Items'ın olmasının sebebi ilgili item'ların ödemesinde bir sıkıntı olduğunda hangi ürünlerin stoklarının eski haline getirilmesi gerektiğini belirlememizdendir.

    }
}