using MassTransit;
using SharedLib;
using SharedLib.Events;
using SharedLib.Interfaces;

namespace SagaStateMachineWorkerService.Models
{
    public class OrderStateMachine : MassTransitStateMachine<OrderStateInstance>
    {
        public Event<IOrderCreatedRequestEvent> OrderCreatedRequestEvent { get; set; } //Bu event geldiğinde State'ini OrderCreated Olarak değiştireceğiz.
        public Event<IStockReservedEvent> StockReservedEvent { get; set; }
        public Event<IPaymentCompletedEvent> PaymentCompletedEvent { get; set; }
        public State OrderCreated { get; private set; } //Bu class dışında set edilmeyecek.
        public State StockReserved { get; private set; }
        public State PaymentCompleted { get; private set; }
        public OrderStateMachine()
        {
            InstanceState(x => x.CurrentState);
            Event(() => OrderCreatedRequestEvent,
            y => y.CorrelateBy<int>(x => x.OrderId, z => z.Message.OrderId)
            .SelectId(context => Guid.NewGuid())); //Db'de orderId ile eventten gelen orderId yi karşılaştır varsa herhangi bir şey yapma yoksa yeni bir satır oluştur ve random guid corellationId oluştur.

            Event(() => StockReservedEvent, x => x.CorrelateById(y => y.Message.CorrelationId)); //StockReservedEvent geldiğinde hangi satırın state'ini StockReserved yapacak.

            Event(() => PaymentCompletedEvent, x => x.CorrelateById(y => y.Message.CorrelationId));
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
                    .Publish(context => new OrderCreatedEvent(context.Instance.CorrelationId) { OrderItems = context.Data.OrderItems }) //Stock microservice' i bu eventi dinliyor.
                                                                                                                                        //Bir event fırlattığımızda event state machine'a state güncellemek için döndüğünde bu event hangi satırla(instance) ilgili olduğunu tespit etmesi için corellationId kullanırız.
                    .TransitionTo(OrderCreated)//Transition ile yukarıdaki işlemlerden(request geldi şuan state initial) sonra OrderCreated state'ine geçiş yap.
                    .Then(context =>
                    {
                        Console.WriteLine($"Order Created Request Event After: {context.Instance}");
                    })
                );

            //---------------------------------------
            During(OrderCreated, //OrderCreatedSate'indeyken StockReservedEvent geldiğinde yapılacakları belirtiyoruz.
                When(StockReservedEvent)
                    .TransitionTo(StockReserved)
                    .Send(new Uri($"queue:{RabbitMQSettingsConst.PaymentStockReservedRequestQueueName}"),
                        context => new StockReservedRequestPayment(context.Instance.CorrelationId)
                        {
                            Payment = new PaymentMessage
                            {
                                CardName = context.Instance.CardName,
                                CardNumber = context.Instance.CardNumber,
                                CVV = context.Instance.CVV,
                                Expiration = context.Instance.Expiration,
                                TotalPrice = context.Instance.TotalPrice
                            },
                            OrderItems = context.Data.OrderItems,
                            BuyerId = context.Instance.BuyerId
                        })
                    .Then(context =>
                    {
                        Console.WriteLine($"Stock Reserved Event After: {context.Instance}");
                    })
                );

            //------------------PaymentCompleted---------------------
            During(StockReserved, //StockReservedState'indeyken PaymentCompletedEvent geldiğinde yapılacakları belirtiyoruz.
                When(PaymentCompletedEvent)
                    .TransitionTo(PaymentCompleted)
                    .Publish(context => new OrderRequestCompletedEvent
                    {
                        OrderId = context.Instance.OrderId
                    })
                    .Then(context =>
                    {
                        Console.WriteLine($"Payment Completed Event After: {context.Instance}");
                    })
                    .Finalize()
                );
        }
    }
}