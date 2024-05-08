
using MediatR;

namespace EventSourcing.API.Commands
{
    public record DeleteProductCommand(Guid id) : IRequest;
}