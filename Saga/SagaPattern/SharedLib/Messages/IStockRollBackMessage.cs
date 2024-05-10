namespace SharedLib.Messages
{
    public interface IStockRollBackMessage
    {
        //Compensation işlemi yapılacak.
        public List<OrderItemMessage> OrderItems { get; set; }
    }
}
