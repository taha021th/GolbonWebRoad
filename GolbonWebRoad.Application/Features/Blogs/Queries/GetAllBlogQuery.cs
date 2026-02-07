using GolbonWebRoad.Domain.Entities;
using GolbonWebRoad.Domain.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace GolbonWebRoad.Application.Features.Blogs.Queries
{
    public class GetAllBlogQuery : IRequest<IEnumerable<Blog>>
    {
    }
    public class GetAllBlogQueryHandler : IRequestHandler<GetAllBlogQuery, IEnumerable<Blog>>
    {
        private readonly IUnitOfWork _unitOfWork;

        private readonly ILogger<GetAllBlogQueryHandler> _logger;
        public GetAllBlogQueryHandler(IUnitOfWork unitOfWork, ILogger<GetAllBlogQueryHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }
        public async Task<IEnumerable<Blog>> Handle(GetAllBlogQuery request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("شروع دریافت وبلاگ ها.");
            return await _unitOfWork.BlogRepository.GetAllAsync(true);
        }
    }
}
