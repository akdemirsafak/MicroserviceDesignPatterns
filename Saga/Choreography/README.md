# Saga - Choreography Pattern

Choreography Based Saga pattern tamamen message broker ile event yayınlama ve dinlemektir.
Her mikroservice'in subscribe ve publisher'ı olacak.

RabbitMQ message broker'ı tercih ediyoruz. Mass Transit kütüphanesi ile çalışacağız.

MassTransit lib. ile RabbitMQ, Azure Service Bus, ActiveMQ,Amazon SQS, gRPC, In Memory message broker'lar kullanılabilir.

