using GolbonWebRoad.Domain.Entities;
using GolbonWebRoad.Domain.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace GolbonWebRoad.Application.Features.BlogCategories.Queries
{
    public class GetByIdBlogCategoryQuery : IRequest<BlogCategory>
    {
        public int Id { get; set; }
    }
    public class GetByIdBlogCategoryQueryHandler : IRequestHandler<GetByIdBlogCategoryQuery, BlogCategory>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<GetByIdBlogCategoryQueryHandler> _logger;
        public GetByIdBlogCategoryQueryHandler(IUnitOfWork unitOfWork, ILogger<GetByIdBlogCategoryQueryHandler> logger)
        {
            _unitOfWork=unitOfWork;
            _logger=logger;
        }
        public async Task<BlogCategory> Handle(GetByIdBlogCategoryQuery request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("شروع دریافت دسته بندی بلاگ با شناسه {BlogCategoryId}", request.Id);
            return await _unitOfWork.BlogCategoryRepository.GetByIdAsync(request.Id, false);
        }
    }
}
