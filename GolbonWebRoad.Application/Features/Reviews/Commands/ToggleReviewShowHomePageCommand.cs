using GolbonWebRoad.Application.Exceptions;
using GolbonWebRoad.Domain.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Caching.Memory;

namespace GolbonWebRoad.Application.Features.Reviews.Commands
{
    public class ToggleReviewShowHomePageCommand : IRequest
    {
        public int ReviewId { get; set; }
        public bool IsShowHomePage { get; set; }
    }

    public class ToggleReviewShowHomePageCommandHandler : IRequestHandler<ToggleReviewShowHomePageCommand>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<ToggleReviewShowHomePageCommandHandler> _logger;
        private readonly IMemoryCache _cache;

        public ToggleReviewShowHomePageCommandHandler(IUnitOfWork unitOfWork, ILogger<ToggleReviewShowHomePageCommandHandler> logger, IMemoryCache cache)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
            _cache = cache;
        }

        public async Task Handle(ToggleReviewShowHomePageCommand request, CancellationToken cancellationToken)
        {
            var review = await _unitOfWork.ReviewsRepository.GetByIdAsync(request.ReviewId);
            if (review == null)
            {
                throw new NotFoundException($"نظر با شناسه {request.ReviewId} یافت نشد.");
            }

            review.IsShowHomePage = request.IsShowHomePage;
            _unitOfWork.ReviewsRepository.Update(review);
            await _unitOfWork.CompleteAsync();
            _cache.Remove("home:data:v1");

            _logger.LogInformation("وضعیت نمایش در صفحه اصلی برای نظر {ReviewId} به {IsShowHomePage} تغییر کرد.", request.ReviewId, request.IsShowHomePage);
        }
    }
}
