using MediatR;

namespace GolbonWebRoad.Application.Features.Reviews.Commands
{
    public class UpdateReviewStatusCommand : IRequest
    {
        public int ReviewId { get; set; }
        public bool Status { get; set; }
    }

    public class UpdateReviewStatusCommandHandler : IRequestHandler<UpdateReviewStatusCommand>
    {
        private readonly GolbonWebRoad.Domain.Interfaces.IUnitOfWork _unitOfWork;

        public UpdateReviewStatusCommandHandler(GolbonWebRoad.Domain.Interfaces.IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task Handle(UpdateReviewStatusCommand request, CancellationToken cancellationToken)
        {
            var review = await _unitOfWork.ReviewsRepository.GetByIdAsync(request.ReviewId);
            if (review == null)
                throw new GolbonWebRoad.Application.Exceptions.NotFoundException("نظر یافت نشد.");

            review.Status = request.Status;
            _unitOfWork.ReviewsRepository.Update(review);
            await _unitOfWork.CompleteAsync();
        }
    }
}
