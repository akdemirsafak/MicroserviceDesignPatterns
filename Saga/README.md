# Saga - Choreography Pattern

Choreography Based Saga pattern tamamen message broker ile event yayınlama ve dinlemektir.
Her mikroservice'in subscribe ve publisher'ı olacak.

RabbitMQ message broker'ı tercih ediyoruz. Mass Transit kütüphanesi ile çalışacağız.

MassTransit lib. ile RabbitMQ, Azure Service Bus, ActiveMQ,Amazon SQS, gRPC, In Memory message broker'lar kullanılabilir.

# Orchestration Notlar

- Servisler arasındaki tüm transaction merkezi bir yerden yönetilir.(Saga State Machine)
  Saga sTate Machine process'i bir service içerisinde de yer alabilir fakat best practice ayrı olmasıdır.Worker service olabilir. 
- 4'den fazla microservice arasında bir distributed transaction yönetimi için uygundur.
- Asynchronous messaging pattern kullnmak uygundur.
- Transaction yönetimi merkezi olduğu için performance bottleneck(darboğaz) fazladır.

Choreography Pattern'de her microservice'in fail durumunu dinleyeceği event sayısı artar.Orchestration'da ise bu problem ortadan kalkar.

State : Uygulama içerisinde var olan bir durumu tutmamız gerekebilir.

Saga Orchestration pattern'de State Machine State Design Pattern'i kullanmaktadır.Manuel olarak kodlamamıza gerek kalmıyor.


Saga State Machine'in Initial evresi request gelmeden önceki evredir.Initial evresinde distributed transaction'ı başlatır.Final evresine kadar aradaki tüm eventleri gerçekleştirir.
State Machine failed eventleri fırlatıldığında compensable transaction işlemlerini yapar.
Mutlaka async iletişim tercih edilmeli bu durumda da message broker kullanmalıyız.

Automatonymous, Masstransit in Orchenstration pattern'de kullandığı State Machine kütüphanesidir.Masstransit bağımlı değildir kendisi tek başına opensource kütüphanedir.

Automatonymous ile birlikte event state'lerini bir db de tutacağız.Azure table,cosmosdb,mongodb,redis farketmeksizin tutabiliriz.
Bu kütüphaneyi kullandığımızda default olarak initial ve final state'ler gelir.

