using GolbonWebRoad.Application.Exceptions;
using GolbonWebRoad.Domain.Entities;
using GolbonWebRoad.Domain.Interfaces;
using MediatR;

namespace GolbonWebRoad.Application.Features.Reviews.Queries
{
    public class GetByIdReviewQuery : IRequest<Review>
    {
        public int Id { get; set; }
        public bool? joinProduct { get; set; }
    }
    public class GetByIdReviewQueryHandler : IRequestHandler<GetByIdReviewQuery, Review>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetByIdReviewQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork=unitOfWork;
        }
        public async Task<Review> Handle(GetByIdReviewQuery request, CancellationToken cancellationToken)
        {
            var review = await _unitOfWork.ReviewsRepository.GetByIdAsync(request.Id, request.joinProduct);
            if (review == null)
                throw new NotFoundException("کامنت با این شناسه یافت نشد!");
            return review;

        }
    }
}
