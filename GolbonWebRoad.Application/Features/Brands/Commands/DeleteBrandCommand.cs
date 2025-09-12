using GolbonWebRoad.Domain.Interfaces;
using MediatR;

namespace GolbonWebRoad.Application.Features.Brands.Commands
{
    public class DeleteBrandCommand : IRequest
    {
        public int Id { get; set; }

    }
    public class DeleteBrandCommandHandler : IRequestHandler<DeleteBrandCommand>
    {
        private readonly IUnitOfWork _unitOfWork;

        public DeleteBrandCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork=unitOfWork;
        }
        public async Task Handle(DeleteBrandCommand request, CancellationToken cancellationToken)
        {
            await _unitOfWork.BrandRepository.DeleteAsync(request.Id);
            await _unitOfWork.CompleteAsync();

        }
    }
}
