using EventSourcing.API.DbContexts;
using EventSourcing.API.DTOs;
using EventSourcing.API.Queries;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace EventSourcing.API.QueryHandlers
{
    public class GetProductsByUserIdQueryHandler : IRequestHandler<GetProductsByUserIdQuery, List<ProductDto>>
    {
        private readonly AppDbContext _dbContext;

        public GetProductsByUserIdQueryHandler(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<List<ProductDto>> Handle(GetProductsByUserIdQuery request, CancellationToken cancellationToken)
        {
            var products= await _dbContext.Products.Where(x=>x.UserId==request.userId).ToListAsync();
            return products.Select(x=>new ProductDto
            {
                Id=x.Id,
                Name=x.Name,
                Price=x.Price,
                UserId=x.UserId,
                Stock=x.Stock
            }).ToList();
        }
    }
}
