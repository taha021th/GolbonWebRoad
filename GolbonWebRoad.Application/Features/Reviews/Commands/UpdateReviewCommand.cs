using AutoMapper;
using GolbonWebRoad.Domain.Entities;
using GolbonWebRoad.Domain.Interfaces;
using MediatR;

namespace GolbonWebRoad.Application.Features.Reviews.Commands
{
    public class UpdateReviewCommand : IRequest
    {
        public int Id { get; set; }
        public string ReviewText { get; set; }
        public int Rating { get; set; }
        public int ProductId { get; set; }
        public string UserId { get; set; }
    }
    public class UpdateReviewCommandHandler : IRequestHandler<UpdateReviewCommand>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public UpdateReviewCommandHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper=mapper;
        }
        public async Task Handle(UpdateReviewCommand request, CancellationToken cancellationToken)
        {
            var entity = _mapper.Map<Review>(request);
            _unitOfWork.ReviewsRepository.Update(entity);
            await _unitOfWork.CompleteAsync();

        }
    }
}
