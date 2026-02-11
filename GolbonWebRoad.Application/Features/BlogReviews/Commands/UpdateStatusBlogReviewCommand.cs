using GolbonWebRoad.Domain.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace GolbonWebRoad.Application.Features.BlogReviews.Commands
{
    public class UpdateStatusBlogReviewCommand : IRequest
    {
        public int Id { get; set; }
        public bool Status { get; set; }
    }
    public class UpdateStatusBlogReviewCommandHandler : IRequestHandler<UpdateStatusBlogReviewCommand>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<UpdateStatusBlogReviewCommandHandler> _logger;
        public UpdateStatusBlogReviewCommandHandler(IUnitOfWork unitOfWork, ILogger<UpdateStatusBlogReviewCommandHandler> logger)
        {
            _unitOfWork=unitOfWork;
            _logger=logger;
        }
        public async Task Handle(UpdateStatusBlogReviewCommand request, CancellationToken cancellationToken)
        {
            var blogReviewEntity = await _unitOfWork.BlogReviewRepository.GetByIdAsync(request.Id);
            blogReviewEntity.Status= request.Status;
            _unitOfWork.BlogReviewRepository.Update(blogReviewEntity);
            await _unitOfWork.CompleteAsync();
        }
    }
}
