using AutoMapper;
using GolbonWebRoad.Domain.Entities;
using GolbonWebRoad.Domain.Interfaces;
using MediatR;

namespace GolbonWebRoad.Application.Features.Reviews.Queries
{
    public class GetReviewsByProductIdQuery : IRequest<ICollection<Review>>
    {
        public int ProductId { get; set; }
        public bool? JoinUser { get; set; } = true;
    }
    
    public class GetReviewsByProductIdQueryHandler : IRequestHandler<GetReviewsByProductIdQuery, ICollection<Review>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public GetReviewsByProductIdQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<ICollection<Review>> Handle(GetReviewsByProductIdQuery request, CancellationToken cancellationToken)
        {
            var reviews = await _unitOfWork.ReviewsRepository.GetByProductIdAsync(request.ProductId, request.JoinUser);
            return reviews;
        }
    }
}
