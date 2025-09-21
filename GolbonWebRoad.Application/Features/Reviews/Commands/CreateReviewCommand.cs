using AutoMapper;
using GolbonWebRoad.Domain.Entities;
using GolbonWebRoad.Domain.Interfaces;
using MediatR;

namespace GolbonWebRoad.Application.Features.Reviews.Commands
{
    public class CreateReviewCommand : IRequest
    {

        public string ReviewText { get; set; }
        public int Rating { get; set; }
        public int ProductId { get; set; }
        public string UserId { get; set; }
    }
    public class CreateReviewsCommandHandler : IRequestHandler<CreateReviewCommand>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        public CreateReviewsCommandHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }
        public async Task Handle(CreateReviewCommand request, CancellationToken cancellationToken)
        {
            var entity = _mapper.Map<Review>(request);
            _unitOfWork.ReviewsRepository.Add(entity);
            await _unitOfWork.CompleteAsync();
        }
    }

}
