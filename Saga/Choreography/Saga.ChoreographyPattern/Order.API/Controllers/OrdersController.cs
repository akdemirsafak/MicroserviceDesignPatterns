using MassTransit;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Order.API.DTOs;
using Order.API.Models;
using SharedLib;

namespace Order.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OrdersController : ControllerBase
    {
        private readonly AppDbContext _dbContext;
        private readonly IPublishEndpoint _publishEndpoint;

        public OrdersController(AppDbContext dbContext,
        IPublishEndpoint publishEndpoint)
        {
            _dbContext = dbContext;
            _publishEndpoint = publishEndpoint;
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateOrderDTO createOrderDTO)
        {
            var newOrder = new Order.API.Models.Order
            {
                BuyerId = createOrderDTO.BuyerId,
                Address = new Address
                {
                    Line = createOrderDTO.Address.Line,
                    Province = createOrderDTO.Address.Province,
                    District = createOrderDTO.Address.District
                },
                CreatedDate = DateTime.Now,
                Status = OrderStatus.Suspend
            };
            createOrderDTO.Items.ForEach(item =>
            {
                newOrder.Items.Add(new OrderItem
                {
                    ProductId = item.ProductId,
                    Count = item.Count,
                    Price = item.Price
                });
            });

            await _dbContext.Orders.AddAsync(newOrder);
            await _dbContext.SaveChangesAsync();

            var orderCreatedEvent = new OrderCreatedEvent
            {
                OrderId = newOrder.Id,
                BuyerId = newOrder.BuyerId,
                Payment = new PaymentMessage
                {
                    CardName = createOrderDTO.Payment.CardName,
                    CardNumber = createOrderDTO.Payment.CardNumber,
                    Expiration = createOrderDTO.Payment.Expiration,
                    CVV = createOrderDTO.Payment.CVV,
                    TotalPrice = newOrder.Items.Sum(x => x.Count * x.Price)
                },
                //TotalPrice = newOrder.Items.Sum(x => x.Count * x.Price),
                OrderItems = newOrder.Items.Select(x => new OrderItemMessage
                {
                    ProductId = x.ProductId,
                    Count = x.Count
                }).ToList()
            };
            await _publishEndpoint.Publish(orderCreatedEvent); //Exchange'e gittiği için bir queue adı vermedik.
            // Publish ve Send farkı :
            // Publish'de eğer bu mesajı subscribe eden yoksa mesaj boşa gider. Publish'de Kimlerin bu mesajı dinlediğini bilmezsiniz. Send'de ise bu message kaydedilir.
            // Publish ile rabbitMq'ya gönderilen event Exchange'e gider. Direkt olarak queue'ya gitmez.
            // Exchange'e gittiğinde bu mesajı dinleyen queue yoksa boşa gider. Kalıcı hale gelmez.!
            // Send'de ise direkt olarak queue'ya gider. Eğer queue yoksa oluşturulur.
            // Publish ile gönderilen mesajlara herhangi bir servis subscribe olabilir fakat Send ile gönderdiğimiz eventler direkt queue'ya gittiği için sadece o kuyruğu dinleyenler alır.
            // Gönderdiğimiz mesajı genelde sadece 1 servis dinleyecekse Send kullanırız.
            // Örneğin ödeme adımında genelde send kullanılır çünkü ödemeyi sadece 1 servis gerçekleştirir.

            return Ok();
        }
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var orders = await _dbContext.Orders.Include(items => items.Items).ToListAsync();
            return Ok(orders);
        }
    }
}