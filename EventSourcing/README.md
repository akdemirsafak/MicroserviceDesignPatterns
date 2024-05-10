
# Event Sourcing Pattern
Microservice mimarilerde çok kullanılan bir design pattern'dir. Yapısal olarak ve çözümsel olarak özel bir patterndir.

##### Nedir?

Dataları bir event olarak "Append only" kaydeden bir patterndir.Her bir event db'ye kaydedilir ve eventler üzerinde bir değişiklik yapılmaz.

Her değişiklik bir eventtir ve event log'a kaydedilir.Event log = Eventleri kaydedeceğimiz database'dir.


Event sourcing finans, lojistik, sağlık, perakende gibi kritik alanlarda kullanımı yaygındır.

Event sourcing için gösterilen en uygun senaryo satranç oyunudur.Kullanıcının her hamlesi kaydedilir ve oyunu kaydedip tekrar açtığında kaldığı yerden devam edebilmelidir.Son adımı kaydetmek yerine tüm adımları kaydeder.Burada X. adıma geri dön diyebiliyor olmalıyız.

Tüm süreci inceleme imkanına sahip oluruz.Önceki değer neydi vs.. Her yaptığını loglama da çözümdür.


E-ticaretteki sepet uygulamaları Event sourcing'e örnektir.Kaç kişi bu ürünü sepete ekledi/sildi gibi tespitler yapabiliriz.Genelde bu senaryolarda kullanılır.

##### Faydaları
1. Audit, event-sourced system büyün eventleri immutable(değiştirilemez) olarak seri halinde kaydeder.
2. Bütün state değişiklikleri kaydedilir.Herhangi bir T zamanına geri dönülebilir. Debugging için çok önemlidir.
3. Veritabanı çökse bile eventler re-play edilerek tekrar entity'lerin güncel durumları elde edilebilir.(Loglamadan ayıran özelliğidir)

##### Özellikleri nelerdir
1. Bir domainin içerisindeki meydana gelen bir gerçeği(gerçek data eventlerdir) temsil eder.
2. Geçmiş zamanı temsil ederler. (Created/Changed gibi.) 
3. Event Sourcing bir event unique bir metadata bilgisine sahiptir(timestamp, identifier) Eventler kronolojik sırayla kaydedilir.

Event Sourcing'in CQRS patternle ile birlikte kullanımı büyük kolaylık sağlar.Cqrs'de Write(Command'ler için kullanılan) db ile Read işlemleri için kullanılan Db arasındaki senkronizasyonu(async) sağlamak için  Event Sourcing kullanılabilir.

#### Konsept

EventStore'da(Eventleri kaydettiğimiz) veriler için okuma yapmayız.Database'imizde datanın son durumu kayıtlı iken EventStore'db sinde datanın tüm geçmişi bulunur(adı x idi y ye güncellendi gibi..)
Eventleri kaydedeceğimiz database herhangi bir db(mssql,mongo vs..) olabilir.

#### Stream
Özel bir domain'e özgü olan event'ler bir stream olarak kaydedilir.
Stream'ler bir nevi tabloya karşılık gelir örneğin Product nesnesinde yaptığımız değişiklikler veritabanına bir akış olarak kaydedilir bu akış(stream) product'ın geçmişinin tutulduğu tablo gibi düşünülebilir.
EventStore'da tablo mantığı yoktur stream mantığıyla işler.
Stream kavramı daha çok eventleri kaydetmek için özel veritabanları için kullanılır.

Event stream'ler domain objeleri için bir kaynaktır ve değişiklik tarihini tutarlar.Bir stream'den event'leri okuyarak güncel state elde edilebilir.
Stream'lerin belirli objeleri temsil eden unique identifier'ları vardır.
Her event'in, stream içerisinde bir pozisyonu bulunur.Bu pozisyon genelde artan bir değerle temsil edilir.
Tüm eventler replay yapılarak ilgili tablo'lar yeniden oluşturulabilir.Bir datann asıl/ham hali EventStore içerisindedir.Query için kullandığımız database sadece EventStore'un yansımasıdır.
Eğer EventStore db çökerse elimizde sadece query db'si(Datanın en güncel hali) kalır fakat Query db çökse bile EventStore'dan eski haline getirilebilir.

### Event Store
1. Eventleri tutmak için özelleştirilmiş veritabanı gibi bir servistir.
2. Opensource'dur cloud olarak ücretli ya da docker ile ücretsiz kullanabilir.
3. Http ve TCP protokolleri üzerinden haberleşilebilir.
4. Eventleri dinleyebilmemiz için bir notification sistemi barındırır. Mongodb, mssql gibi db'lerde bu özellik bulunmaz. 
   </br>
   Örneğin bir product stream'ine bir event yollandığında ve dinlendiğinde event store ilgili stream'e subscribe olduysak bize iletir ve biz db ye yazabiliriz.Bu senaryoda message broker'a da ihtiyacımız kalmaz.Mongodb gibi bir db kullanırsak message broker sisteme ihtiyaç duyarız.

#### Projection

Event based data model'den okuma veya yazma modellerinin oluşturulmasını sağlar.
Ham datanın yorumlanmasıdır.(eventlerin anlamlı bir hale gelmesi.)

Stream'den belirli tarihler arasındaki datanın en güncel halini Projection ile alabiliriz.
Stream'ler ham data iken Projection bu dataların yansımasıdır.

#### Subscription (Önemli)
Event Store içerisine kaydedilen her event bir notification yayınlar, bu notification'lara subscribe olursak belirli işlemler gerçekleştirebiliriz. Örneğin Read-Database'i güncellemek.

Bu yapı yoksa message broker kullanmamız gerekir ve ekstra efor gerektirir.

#### Event Store'da Modeller :

1. Write Model
  DDD kullanılıyorsa, DDD içerisinde eventleri db ye yazma işlemi için en uygun senaryo aggregate'ler içerisinde yapılmasıdır.
2. Read model: Query'lerin nasıl ele alınacağı ile ilgilidir.

<strong> SSL bağlantısına ihtiyacımız olacak ve bu adreste açıklaması var.
https://developers.eventstore.com/server/v5/#getting-started 
https://developers.eventstore.com/server/v5/security.html#setting-up-ssl-for-docker</strong>

localhost:2113
-- Default user : admin 
-- Default password : changeit

Db hazırlandıktan sonra Eventleri planlamak tamamıyla bizim tercihlerimize bağlıdır.
ReadDb'de bir sıkıntı çıktığında ya da kaybolduğunda -gerektiğinde- eventstore'dan replay yapabilmemiz için id alanını kendimiz belirliyoruz.

## Subscribe Türleri

### Volatile Subscriptions
 Product stream'de 100 event olduğunu varsayalım.Bu subscription yöntemi ile 100. den sonra gelen eventleri dinlemiş oluruz.
 Subscribe olduğumuzda hangi eventleri dinleyeceğimizi kendimiz seçmemiz gerekir.Bir bağlantı kopukluğunda aradaki eventler kayıp olur.

### Catch up Subscriptions
 Herhangi bir stream'e subscribe olduğumuzda en baştan dinliyoruz, bağlantı kopukluğunda tekrar 0 dan dinlemeye başlar.Dinleyen olarak ayrıca başka bir yere hangi eventi dinlediğimizi de kaydetmemiz gerekir. 

### Persistance Subscription
Hangi eventte kaldığımızı eventstore takip eder.Bağlantı koptuğunda veya process çöktüğünde tekrar bağlandığımızda eventstore hangi eventte kaldıysa ordan devam eder.

Subscribe olayını gerçekleştirmek için Panelden(localhost:2113) Persistent Subscriptions içerisine yeni bir subscrition oluştururuz.
Group => Birden fazla client aynı subscribe'da gruplandırırsa EventStore MessageBroker gibi çalışır.Eventleri sırayla yollar.
Min CheckPoint Count => Örneğin 10 olarak belirlediğimizde Dashboard 10 event gönderildikten sonra güncellenir.



### Replay
1. Panelden Yeni bir subscription oluşturulur.(Aynı adla grubu başka)
2. ProductStream.cs içerisinde grup adını yeni adla güncelleyip projeyi run ettiğimizde dataları Read db ye ekleyecektir.
