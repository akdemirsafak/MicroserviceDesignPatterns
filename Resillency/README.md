# Resiliency

Mikro servislerimizde partial failure'lar meydana gelmesi olağandır.Bu durumda uygulamamız bu durumu tolere edebilir olmalıdır.
Amacı : Kısmi bir hata meydana geldikten sonra uygulamanın çalışmasına aynen devam etmesidir.
Uygulamalar mutlaka resilient olmak zorundadır.
Failure örnekleri : Network Bottleneck, VM crash.Bir cluster içerisinde farklı bir noda taşınan mikoservisler kısa süreliğine failure olur.

Herhangi bir pattern kullanmadan önce servis mimarisinin gözden geçirilmesi gerekir.Servisin kısmi hatalara karşı direncinin ve esnekliğinin yüksek olması için servisler arası async veya sync iletişimlere dikkat edilmelidir.
Client ile iletişime geçen mikroservislerin client ile arasındaki iletişim senkron, mikroservisler arasındaki iletişim ise asenkron olmalıdır.

Servisler arası senkron işlem yapılıyorsa, X servisinden Y servisine istek yaptığımızda Y servisi down olduğunda yapılan istek boşa gider.

EfCore Resiliency
Execution strategy'e sahip bir db kullanılıyorsa otomatik bir şekilde başarısız olan command'leri retry eder.
Ef Core Sql Server'da başarısız olan exception'ları bilip en uygun default retry sayısını ve gecikme süresini hesaplar.
EfCore bu işlemleri yapabilmek için ResultSet'leri buffer'layacaktır. Bu da Memory  tüketiminin artacağı anlamına gelir.
EnableRetryOnFailure() methodu OnConfiguring'da aktifleştirilebilir.

### Retry Pattern

Retry Pattern kısa süre içerisinde up olacağını bildiğimiz servisler için kullanılmalıdır.



### Circuit Braker Pattern 