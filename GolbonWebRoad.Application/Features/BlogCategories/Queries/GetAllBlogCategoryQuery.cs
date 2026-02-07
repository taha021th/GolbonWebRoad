using GolbonWebRoad.Domain.Entities;
using GolbonWebRoad.Domain.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace GolbonWebRoad.Application.Features.BlogCategories.Queries
{
    public class GetAllBlogCategoryQuery : IRequest<IEnumerable<BlogCategory>>
    {
    }
    public class GetAllBlogCategoryQueryHandler : IRequestHandler<GetAllBlogCategoryQuery, IEnumerable<BlogCategory>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<GetAllBlogCategoryQueryHandler> _logger;
        public GetAllBlogCategoryQueryHandler(IUnitOfWork unitOfWork, ILogger<GetAllBlogCategoryQueryHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _logger=logger;
        }
        public async Task<IEnumerable<BlogCategory>> Handle(GetAllBlogCategoryQuery request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("شروع دریافت دسته بندی های بلاگ");
            var entity = await _unitOfWork.BlogCategoryRepository.GetAllAsync();
            return entity;
        }
    }
}
