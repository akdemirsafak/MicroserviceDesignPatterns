using EventSourcing.API.Commands;
using EventSourcing.API.EventStores;
using MediatR;

namespace EventSourcing.API.CommandHandlers
{
    public class ChangeProductNameCommandHandler : IRequestHandler<ChangeProductNameCommand>
    {
        private readonly ProductStream _productStream;

        public ChangeProductNameCommandHandler(ProductStream productStream)
        {
            _productStream = productStream;
        }

        public async Task<Unit> Handle(ChangeProductNameCommand request, CancellationToken cancellationToken)
        {
            _productStream.NameChanged(request.ChangeProductNameDto);
            await _productStream.SaveAsync();
            return Unit.Value;
        }
    }
}