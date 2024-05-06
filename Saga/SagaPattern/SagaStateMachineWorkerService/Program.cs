using MassTransit;
using Microsoft.EntityFrameworkCore;
using SagaStateMachineWorkerService;
using SagaStateMachineWorkerService.Models;
using SharedLib;
using System.Reflection;

var builder = Host.CreateApplicationBuilder(args);
builder.Services.AddHostedService<Worker>();

builder.Services.AddMassTransit(cfg =>
{

    cfg.AddSagaStateMachine<OrderStateMachine, OrderStateInstance>().EntityFrameworkRepository(opt =>
    {
        opt.AddDbContext<DbContext, OrderStateDbContext>((provider, options) =>
        {
            options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"),
            m => m.MigrationsAssembly(Assembly.GetExecutingAssembly().GetName().Name));
        });
    });

    cfg.AddBus(provider => Bus.Factory.CreateUsingRabbitMq(configure =>
    {
        configure.Host(builder.Configuration.GetConnectionString("RabbitMQ"));
        // Add your consumers here like
        configure.ReceiveEndpoint(RabbitMQSettingsConst.OrderSaga, e =>
        {
            e.ConfigureSaga<OrderStateInstance>(provider);
        });

    }));
});



var host = builder.Build();
host.Run();
