namespace SharedLib
{
    public class PaymentSucceededEvent
    {
        public int OrderId { get; set; }
        public string BuyerId { get; set; } // OrderId'den satın alan kullanıcıyı görüntüleyebiliriz.Fakat başka microservisler de bu eventi dinleyebiliyor olabilir. Bu yüzden BuyerId'yi de gönderiyoruz.
    }
}
