
namespace SharedLib.Messages
{
    public class StockRollBackMessage : IStockRollBackMessage
    {
        public List<OrderItemMessage> OrderItems { get; set; } = new();
    }
}
