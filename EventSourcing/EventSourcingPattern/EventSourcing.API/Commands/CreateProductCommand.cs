using EventSourcing.API.DTOs;
using MediatR;

namespace EventSourcing.API.Commands
{
    public record CreateProductCommand(CreateProductDto CreateProductDto) : IRequest;
}