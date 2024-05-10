using EventSourcing.API.Commands;
using EventSourcing.API.DTOs;
using EventSourcing.API.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace EventSourcing.API.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class ProductController : ControllerBase
    {
        private readonly IMediator _mediatr;

        public ProductController(IMediator mediatr)
        {
            _mediatr = mediatr;
        }
        [HttpGet("{userId}")]
        public async Task<IActionResult> GetProductsByUserId(int userId)
        {
            var products = await _mediatr.Send(new GetProductsByUserIdQuery(userId));
            return Ok(products);
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateProductDto createProductDto)
        {
            await _mediatr.Send(new CreateProductCommand(createProductDto));
            return Created();
        }
        [HttpPut]
        public async Task<IActionResult> ChangeName(ChangeProductNameDto changeProductNameDto)
        {
            await _mediatr.Send(new ChangeProductNameCommand(changeProductNameDto));
            return NoContent();
        }
        [HttpPut]
        public async Task<IActionResult> ChangePrice(ChangeProductPriceDto changeProductPriceDto)
        {
            await _mediatr.Send(new ChangeProductPriceCommand(changeProductPriceDto));
            return NoContent();
        }
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            await _mediatr.Send(new DeleteProductCommand(id));
            return NoContent();
        }
    }
}