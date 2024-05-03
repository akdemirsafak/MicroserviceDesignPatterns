namespace Order.API.Models
{
    public class Order
    {
        public int Id { get; set; }
        public string CustomerId { get; set; }
        public OrderStatus Status { get; set; }
        public ICollection<OrderItem> Items { get; set; } = new List<OrderItem>();
        public Address Address { get; set; }

        public string? FailMessage { get; set; }
    }
    public enum OrderStatus
    {
        Suspend, Success, Fail
    }
}