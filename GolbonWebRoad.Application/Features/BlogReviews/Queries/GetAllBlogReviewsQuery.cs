using GolbonWebRoad.Domain.Entities;
using GolbonWebRoad.Domain.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace GolbonWebRoad.Application.Features.BlogReviews.Queries
{
    public class GetAllBlogReviewsForAdminQuery : IRequest<IEnumerable<BlogReview>>
    {
        public bool? Status { get; set; }
    }
    public class GetAllBlogReviewsForAdminQueryHandler : IRequestHandler<GetAllBlogReviewsForAdminQuery, IEnumerable<BlogReview>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<GetAllBlogReviewsForAdminQueryHandler> _logger;
        public GetAllBlogReviewsForAdminQueryHandler(IUnitOfWork unitOfWork, ILogger<GetAllBlogReviewsForAdminQueryHandler> logger)
        {
            _unitOfWork=unitOfWork;
            _logger=logger;
        }

        public async Task<IEnumerable<BlogReview>> Handle(GetAllBlogReviewsForAdminQuery request, CancellationToken cancellationToken)
        {
            return await _unitOfWork.BlogReviewRepository.GetBlogReviewsWithDetailAsync(request.Status);
        }
    }
}
