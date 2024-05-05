using MassTransit;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Order.API.DTOs;
using Order.API.Models;
using SharedLib;
using SharedLib.Events;
using SharedLib.Interfaces;

namespace Order.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OrdersController : ControllerBase
    {
        private readonly AppDbContext _dbContext;
        private readonly ISendEndpointProvider _sendEndpointProvider;

        public OrdersController(AppDbContext dbContext,

        ISendEndpointProvider sendEndpointProvider)
        {
            _dbContext = dbContext;
            _sendEndpointProvider = sendEndpointProvider;
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

            #region 
            var orderCreatedEvent = new OrderCreatedRequestEvent
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
                OrderItems = newOrder.Items.Select(x => new OrderItemMessage
                {
                    ProductId = x.ProductId,
                    Count = x.Count
                }).ToList()
            };
            #endregion


            var sendEndpoint = await _sendEndpointProvider.GetSendEndpoint(new Uri($"queue:{RabbitMQSettingsConst.OrderSaga}"));
            //Eğer event'i publish edersek o anda State Machine down ise mesajlar boşa gider.Send ile gönderirsek direkt queue'ya yazılır.

            await sendEndpoint.Send<IOrderCreatedRequestEvent>(orderCreatedEvent);

            return Ok();
        }
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var orders = await _dbContext.Orders.ToListAsync();
            return Ok(orders);
        }
    }
}