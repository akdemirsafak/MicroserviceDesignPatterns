namespace Order.API.DTOs
{
    public class CreateOrderDTO
    {
        public string BuyerId { get; set; }
        public List<OrderItemDTO> Items { get; set; } = new();
        public PaymentDTO Payment { get; set; }
        public AddressDTO Address { get; set; }
    }

    #region OrderItemDTO
    public class OrderItemDTO
    {
        public int ProductId { get; set; }
        public int Count { get; set; }
        public decimal Price { get; set; }
    }
    #endregion
    #region PaymentDTO
    public class PaymentDTO
    {
        public string CardName { get; set; }
        public string CardNumber { get; set; }
        public string Expiration { get; set; }
        public string CVV { get; set; }
    }
    #endregion
    #region AddressDTO
    public class AddressDTO
    {
        public string Line { get; set; }
        public string Province { get; set; }
        public string District { get; set; }
    }
    #endregion
}