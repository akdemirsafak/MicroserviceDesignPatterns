using MassTransit;
using SharedLib.Interfaces;

namespace SagaStateMachineWorkerService.Models
{
    public class OrderStateMachine : MassTransitStateMachine<OrderStateInstance>
    {
        public Event<IOrderCreatedRequestEvent> OrderCreatedRequestEvent { get; set; } //Bu event geldiğinde State'ini OrderCreated Olarak değiştireceğiz.
        public State OrderCreated { get; private set; } //Bu class dışında set edilmeyecek.
        public OrderStateMachine()
        {
            InstanceState(x => x.CurrentState);
            Event(() => OrderCreatedRequestEvent,
            y => y.CorrelateBy<int>(x => x.OrderId, z => z.Message.OrderId)
            .SelectId(context => Guid.NewGuid())); //Db'de orderId ile eventten gelen orderId yi karşılaştır varsa herhangi bir şey yapma yoksa yeni bir satır oluştur ve random guid corellationId oluştur.

            //Initial state'den sonraki state'e geçerken yapılacakları belirtiyoruz.
            Initially(
                When(OrderCreatedRequestEvent)
                    .Then(context =>
                    {
                        //Instance'lar db deki context'ler ise eventteki dataları temsil eder.
                        context.Instance.BuyerId = context.Data.BuyerId;
                        context.Instance.OrderId = context.Data.OrderId;
                        context.Instance.CardName = context.Data.Payment.CardName;
                        context.Instance.TotalPrice = context.Data.Payment.TotalPrice;
                        context.Instance.CardNumber = context.Data.Payment.CardNumber;
                        context.Instance.CVV = context.Data.Payment.CVV;
                        context.Instance.Expiration = context.Data.Payment.Expiration;
                        context.Instance.CreatedDate = DateTime.Now;
                    })
                    .Then(context => Console.WriteLine($"Order Created Request Event Before: {context.Instance}"))
                    .TransitionTo(OrderCreated)//Transition ile yukarıdaki işlemlerden(request geldi şuan state initial) sonra OrderCreated state'ine geçiş yap.
                    .Then(context =>
                    {
                        Console.WriteLine($"Order Created Request Event After: {context.Instance}");
                    }));
        }
    }
}