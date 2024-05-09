using Microsoft.AspNetCore.Mvc;

namespace ServiceB.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductsController : ControllerBase
{
    [HttpGet("{id}")]
    public async Task<IActionResult> Get(int id)
    {
        return Ok(new { Id = id, Name = "Kalem", Stock = 5, Price = 123.5m });
    }
}