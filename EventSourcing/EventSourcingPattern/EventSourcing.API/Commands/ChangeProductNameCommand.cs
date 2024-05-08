using EventSourcing.API.DTOs;
using MediatR;

namespace EventSourcing.API.Commands
{
    public record ChangeProductNameCommand(ChangeProductNameDto ChangeProductNameDto) : IRequest;
}