using GolbonWebRoad.Domain.Entities;
using GolbonWebRoad.Domain.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace GolbonWebRoad.Application.Features.Blogs.Queries
{
    public class GetByIdBlogQuery : IRequest<Blog>
    {
        public int Id { get; set; }
    }
    public class GetByIdBlogQueryHandler : IRequestHandler<GetByIdBlogQuery, Blog>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<GetByIdBlogQueryHandler> _logger;

        public GetByIdBlogQueryHandler(IUnitOfWork unitOfWork, ILogger<GetByIdBlogQueryHandler> logger)
        {
            _unitOfWork=unitOfWork;
            _logger=logger;
        }

        public async Task<Blog> Handle(GetByIdBlogQuery request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("شروع دریافت وبلاگ با شناسه {BlogId}.", request.Id);
            return await _unitOfWork.BlogRepository.GetByIdAsync(request.Id, true, true, true);
        }
    }

}
