using GolbonWebRoad.Domain.Interfaces;
using MediatR;

namespace GolbonWebRoad.Application.Features.Products.ProductAttributeValues.Commands
{
    public class DeleteProductValueCommand : IRequest
    {
        public int Id { get; set; }
    }
    public class DeleteProductValueCommandHandler : IRequestHandler<DeleteProductValueCommand>
    {
        private readonly IUnitOfWork _unitOfWork;

        public DeleteProductValueCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork=unitOfWork;
        }
        public async Task Handle(DeleteProductValueCommand request, CancellationToken cancellationToken)
        {
            await _unitOfWork.ProductAttributeValueRepository.RemoveAsync(request.Id);
            await _unitOfWork.CompleteAsync();
        }
    }
}
