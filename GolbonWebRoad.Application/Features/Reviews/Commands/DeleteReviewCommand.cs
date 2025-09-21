using GolbonWebRoad.Domain.Interfaces;
using MediatR;

namespace GolbonWebRoad.Application.Features.Reviews.Commands
{
    public class DeleteReviewCommand : IRequest
    {
        public int Id { get; set; }
    }
    public class DeleteReviewCommandHandler : IRequestHandler<DeleteReviewCommand>
    {
        private readonly IUnitOfWork _unitOfWork;

        public DeleteReviewCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork=unitOfWork;
        }

        public async Task Handle(DeleteReviewCommand request, CancellationToken cancellationToken)
        {
            await _unitOfWork.ReviewsRepository.DeleteAsync(request.Id);
            await _unitOfWork.CompleteAsync();
        }
    }
}
