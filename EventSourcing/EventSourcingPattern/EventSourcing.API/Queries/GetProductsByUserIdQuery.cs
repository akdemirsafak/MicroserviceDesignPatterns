using EventSourcing.API.DTOs;
using MediatR;

namespace EventSourcing.API.Queries
{
    public record GetProductsByUserIdQuery(int userId) : IRequest<List<ProductDto>>;
}
