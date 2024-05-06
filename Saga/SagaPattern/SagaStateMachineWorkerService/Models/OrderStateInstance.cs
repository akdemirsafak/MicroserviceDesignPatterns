using MassTransit;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace SagaStateMachineWorkerService.Models
{
    public class OrderStateInstance : SagaStateMachineInstance
    {
        public Guid CorrelationId { get; set; }

        //CorreltionId her state machine instance'ı için(db'de bir satır olarak tutulacak) random değer üretir.
        //State Machine'in requestleri ayırt etmek için kullandığı id'dir.

        public string CurrentState { get; set; }
        //Initial,OrderCreated,StockReserved,StockNotReserved,PaymentCompleted,PaymentFailed,Final state'lerini tutmak için.
        //Initial ve Final state'leri default olarak kütüphaneden gelecektir.
        public string BuyerId { get; set; }
        public int OrderId { get; set; }
        public string CardName { get; set; }
        public string CardNumber { get; set; }
        public string Expiration { get; set; }
        public string CVV { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public decimal TotalPrice { get; set; }
        public DateTime CreatedDate { get; set; }
        public override string ToString()
        {
            var properties = GetType().GetProperties();
            var stringBuilder = new StringBuilder();
            foreach (var property in properties)
            {
                var value = property.GetValue(this, null); //index null
                stringBuilder.Append($"{property.Name} : {value}");
            }
            stringBuilder.Append("--------------");
            return stringBuilder.ToString();
        }

    }
}