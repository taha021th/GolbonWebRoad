using AutoMapper;
using GolbonWebRoad.Domain.Entities;
using GolbonWebRoad.Domain.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace GolbonWebRoad.Application.Features.BlogReviews.Commands
{
    public class CreateBlogReviewCommand : IRequest
    {
        public string ReviewText { get; set; }
        public int Rating { get; set; }
        public int BlogId { get; set; }
        public string UserId { get; set; }
        public ApplicationUser User { get; set; }


    }
    public class CreateBlogReviewCommandHandler : IRequestHandler<CreateBlogReviewCommand>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<CreateBlogReviewCommand> _logger;
        public CreateBlogReviewCommandHandler(IUnitOfWork unitOfWork, IMapper mapper, ILogger<CreateBlogReviewCommand> logger)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger=logger;
        }
        public async Task Handle(CreateBlogReviewCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("در حال ایجاد کامنت برای بلاگ با شناسه {BlogId}", request.BlogId);
            var entity = _mapper.Map<BlogReview>(request);
            entity.ReviewDate=DateTime.UtcNow;
            await _unitOfWork.BlogReviewRepository.AddAsync(entity);
            await _unitOfWork.CompleteAsync();
        }
    }
}
