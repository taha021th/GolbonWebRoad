using MediatR;
using Microsoft.Extensions.Caching.Memory;

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
        private readonly IMemoryCache _cache;

        public UpdateReviewStatusCommandHandler(GolbonWebRoad.Domain.Interfaces.IUnitOfWork unitOfWork, IMemoryCache cache)
        {
            _unitOfWork = unitOfWork;
            _cache = cache;
        }

        public async Task Handle(UpdateReviewStatusCommand request, CancellationToken cancellationToken)
        {
            var review = await _unitOfWork.ReviewsRepository.GetByIdAsync(request.ReviewId);
            if (review == null)
                throw new GolbonWebRoad.Application.Exceptions.NotFoundException("نظر یافت نشد.");

            review.Status = request.Status;
            _unitOfWork.ReviewsRepository.Update(review);
            await _unitOfWork.CompleteAsync();
            _cache.Remove("home:data:v1");
        }
    }
}
