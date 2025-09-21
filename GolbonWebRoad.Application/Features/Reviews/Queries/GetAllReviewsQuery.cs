using AutoMapper;
using GolbonWebRoad.Domain.Entities;
using GolbonWebRoad.Domain.Interfaces;
using MediatR;

namespace GolbonWebRoad.Application.Features.Reviews.Queries
{
    public class GetAllReviewsQuery : IRequest<ICollection<Review>>
    {
        public bool? joinProduct { get; set; }
        public int Take { get; set; } = 0;
    }
    public class GetAllReviewsQueryHandler : IRequestHandler<GetAllReviewsQuery, ICollection<Review>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public GetAllReviewsQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)

        {
            _unitOfWork=unitOfWork;
            _mapper=mapper;
        }

        public async Task<ICollection<Review>> Handle(GetAllReviewsQuery request, CancellationToken cancellationToken)
        {
            var reviews = await _unitOfWork.ReviewsRepository.GetAllAsync(request.joinProduct, request.Take);
            return reviews;
        }
    }
}
