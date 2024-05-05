namespace Order.API.Models
{
    public class Order
    {
        public int Id { get; set; }
        public string BuyerId { get; set; }
        public DateTime CreatedDate { get; set; }
        public OrderStatus Status { get; set; }
        public virtual ICollection<OrderItem> Items { get; set; } = new List<OrderItem>();
        public Address Address { get; set; }

        public string? FailMessage { get; set; }
    }
    public enum OrderStatus
    {
        Suspend, Completed, Failed
    }
}