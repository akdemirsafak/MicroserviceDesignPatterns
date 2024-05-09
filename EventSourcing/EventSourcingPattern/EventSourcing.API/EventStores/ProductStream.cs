using EventSourcing.API.DTOs;
using EventSourcing.Shared.Events;
using EventStore.ClientAPI;

namespace EventSourcing.API.EventStores
{
    public class ProductStream : AbstractStream
    {
        public static string StreamName => "product";
        public static string GroupName => "AGroup";
        public ProductStream(IEventStoreConnection eventStoreConnection) : base(StreamName, eventStoreConnection)
        {
        }

        public async Task CreatedAsync(CreateProductDto productDto)
        {
            Events.AddLast(new ProductCreatedEvent()
            {
                Id = Guid.NewGuid(),
                Name = productDto.Name,
                Price = productDto.Price,
                Stock = productDto.Stock,
                UserId = productDto.UserId
            }); //AddLast LinkedList'ten gelir.
        }
        public void NameChanged(ChangeProductNameDto changeProductNameDto)
        {
            Events.AddLast(new ProductNameChangedEvent()
            {
                Id = changeProductNameDto.Id,
                Name = changeProductNameDto.Name
            });
        }
        public void PriceChanged(ChangeProductPriceDto changeProductPriceDto)
        {
            Events.AddLast(new ProductPriceChangedEvent()
            {
                Id = changeProductPriceDto.Id,
                ChangedPrice = changeProductPriceDto.Price
            });
        }
        public void Deleted(Guid id)
        {
            Events.AddLast(new ProductDeletedEvent()
            {
                Id = id
            });
        }
    }
}