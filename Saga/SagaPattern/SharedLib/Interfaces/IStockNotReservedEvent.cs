using MassTransit;

namespace SharedLib.Interfaces
{
    public interface IStockNotReservedEvent : CorrelatedBy<Guid>
    {
        public string Reason { get; set; }

    }
}
