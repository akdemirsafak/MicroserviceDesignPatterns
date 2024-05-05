using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace SagaStateMachineWorkerService.Models
{
    public class OrderStateMap : SagaClassMap<OrderStateInstance>
    {
        protected override void Configure(EntityTypeBuilder<OrderStateInstance> entity, ModelBuilder model)
        {
            entity.Property(x => x.BuyerId).IsRequired().HasMaxLength(256);
            entity.Property(x => x.CardName).HasMaxLength(256);

            base.Configure(entity, model);
        }
    }
}