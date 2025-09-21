using AutoMapper;
using GolbonWebRoad.Domain.Entities;
using GolbonWebRoad.Domain.Interfaces;
using MediatR;

namespace GolbonWebRoad.Application.Features.Reviews.Queries
{
    public class GetAllReviewsWithDetailsQuery : IRequest<List<Review>>
    {
        public bool? Status { get; set; } // null = all, true = approved, false = pending
    }

    public class GetAllReviewsWithDetailsQueryHandler : IRequestHandler<GetAllReviewsWithDetailsQuery, List<Review>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetAllReviewsWithDetailsQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<List<Review>> Handle(GetAllReviewsWithDetailsQuery request, CancellationToken cancellationToken)
        {
            var reviews = await _unitOfWork.ReviewsRepository.GetReviewsWithDetailsAsync(request.Status);
            return reviews.ToList();
        }
    }
}
