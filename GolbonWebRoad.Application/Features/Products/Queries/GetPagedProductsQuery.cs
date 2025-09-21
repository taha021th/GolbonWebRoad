using GolbonWebRoad.Domain.Entities;
using GolbonWebRoad.Domain.Interfaces;
using GolbonWebRoad.Domain.Interfaces.Repositories; // Add this using for PagedResult<T>
using MediatR;
using Microsoft.Extensions.Logging;

namespace GolbonWebRoad.Application.Features.Products.Queries
{
    public class GetPagedProductsQuery : IRequest<PagedResult<Product>> // Return the Domain Entity
    {
        public string? SearchTerm { get; set; }
        public int? CategoryId { get; set; }
        public int? BrandId { get; set; }
        public string? SortOrder { get; set; }
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 9;
    }

    public class GetPagedProductsQueryHandler : IRequestHandler<GetPagedProductsQuery, PagedResult<Product>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<GetPagedProductsQueryHandler> _logger;

        // IMapper is no longer needed here
        public GetPagedProductsQueryHandler(IUnitOfWork unitOfWork, ILogger<GetPagedProductsQueryHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<PagedResult<Product>> Handle(GetPagedProductsQuery request, CancellationToken cancellationToken)
        {
            _logger.LogInformation(
                "Fetching paged product entities with filters: Search='{SearchTerm}', CategoryId={CategoryId}, BrandId={BrandId}, Page={PageNumber}",
                request.SearchTerm, request.CategoryId, request.BrandId, request.PageNumber);

            try
            {
                var pagedProducts = await _unitOfWork.ProductRepository.GetPagedProductsAsync(
                    request.PageNumber,
                    request.PageSize,
                    request.SearchTerm,
                    request.CategoryId,
                    request.BrandId,
                    request.SortOrder);

                if (pagedProducts == null || !pagedProducts.Items.Any())
                {
                    _logger.LogWarning("No product entities found for the specified filters.");
                    return new PagedResult<Product>
                    {
                        Items = new List<Product>(),
                        PageNumber = request.PageNumber,
                        PageSize = request.PageSize,
                        TotalCount = 0
                    };
                }

                _logger.LogInformation("Successfully fetched {ProductCount} product entities.", pagedProducts.Items.Count);

                // Return the raw entity result directly
                return pagedProducts;
            }
            catch (Exception ex)
            {
                _logger.LogCritical(ex, "A critical error occurred while fetching paged product entities.");
                throw;
            }
        }
    }
}

