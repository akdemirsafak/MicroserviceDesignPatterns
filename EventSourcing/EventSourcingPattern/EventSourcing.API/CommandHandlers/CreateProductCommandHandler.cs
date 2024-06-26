using EventSourcing.API.Commands;
using EventSourcing.API.EventStores;
using MediatR;

namespace EventSourcing.API.CommandHandlers
{
    public class CreateProductCommandHandler : IRequestHandler<CreateProductCommand>
    {
        private readonly ProductStream _productStream;

        public CreateProductCommandHandler(ProductStream productStream)
        {
            _productStream = productStream;
        }

        public async Task<Unit> Handle(CreateProductCommand request, CancellationToken cancellationToken)
        {
            await _productStream.CreatedAsync(request.CreateProductDto);
            await _productStream.SaveAsync();
            return Unit.Value;
        }
    }
}