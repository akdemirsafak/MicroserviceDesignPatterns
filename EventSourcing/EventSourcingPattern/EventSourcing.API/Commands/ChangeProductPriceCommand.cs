using EventSourcing.API.DTOs;
using MediatR;

namespace EventSourcing.API.Commands
{
    public record ChangeProductPriceCommand(ChangeProductPriceDto ChangeProductPriceDto) : IRequest;
}