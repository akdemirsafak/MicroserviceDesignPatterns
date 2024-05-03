# Microservice Design Pattern

<h2> Saga Pattern</h2>

<h3> Distributed Transaction</h3>
    Servisler arasında veri tutarlılığını yönetmeye imkan veren pattern'dir.

Örnek: Ürünlerin ve siparişlerin farklı servisler tarafından yönetildiği senaryoda; sipariş verildiğinde ürün stoğunun azalmaması durumudur.

<h4>Transaction Nedir?</h4> 

İşlem birimleri diyebiliriz. Bir veya birden fazla operasyondan oluşabilir.
Para transferi örnek verilebilir: X Kişisinin hesabından parayı çek(1. Transaction) Y kişisinin hesabına yatır(2. transaction)
Burada işlem esnasında çıkacak bir problem bütünlüğü bozabilir.

<h4> ACID nedir?</h4>
Değişikliklerin db'ye nasıl uygulanacağını belirten prensiptir.
- Atomik : Ya Hep Ya hiç prensibidir.
- Consistency : Dataların tutarlı olması. Veritabanını sürekli valid tutar.
- Isolation : Transaction'ların birbirinden bağımsız olmasını ifade eder.
- Durability : Dataların güvenli bir ortamda saklanmasını ifade eder.
  

<h3> Saga Implementations</h3>

<h4>Choreography based </h4>
Daha kolay olan yöntemdir.

1. Distributed transaction'a katılacak servis sayısı maksimum 4 ise uygulanması önerilir.
2. Sisteme katılan her katılımcı bir karar vericidir.(Başarılı-başarısız)
3. Herhangi bir merkezi yöntem yoktur.
4. Senkron ve asenkron iletişim yapılabilir. Tercih edilmesi gereken message broker ile async iletişimdir.
5. Her servis kuyruğu dinler, gelen event/message ile ilgili işlem yapar ve sonuç başarılı veya başarısız durumunu tekrar kuyruğa döner.
6. Point to Point bir iletişim olmadığı için servisler arası bağlılık azalır.

Biz senaryomuzda Async iletişimi tercih edeceğiz.Buraya message broker kullanacağız. 
Senkron iletişim servislerin birbirinin endpointlere istek yapması durumudur.

<h5>Compensable Transactions</h5>
Bir transaction'ın yapmış olduğu işlemi tersine alan transaction'lardır. 
Örnek : Ürün stoktan düşüldükten sonra ödeme başarısız oldu burada önceki adıma gider ve stoktan düşülen ürünü eski haline getirir.

***********
Event'ler geçmiş zamanı, mesajlar gelecek zamanı ifade ederler bu şekilde isimlendirilirler.
