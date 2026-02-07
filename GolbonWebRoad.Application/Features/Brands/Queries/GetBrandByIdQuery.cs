using GolbonWebRoad.Domain.Entities;
using GolbonWebRoad.Domain.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace GolbonWebRoad.Application.Features.Brands.Queries
{
    public class GetBrandByIdQuery : IRequest<Brand>
    {
        public int Id { get; set; }
        public bool joinProduct { get; set; }
    }
    public class GetBrandByIdQueryHandler : IRequestHandler<GetBrandByIdQuery, Brand>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<GetBrandByIdQueryHandler> _logger;

        public GetBrandByIdQueryHandler(IUnitOfWork unitOfWork, ILogger<GetBrandByIdQueryHandler> logger)
        {
            _unitOfWork=unitOfWork;
            _logger=logger;

        }
        public async Task<Brand> Handle(GetBrandByIdQuery request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("شروع دریافت برند با شناسه {BrandId}", request.Id);
            return await _unitOfWork.BrandRepository.GetByIdAsync(request.Id, request.joinProduct);
        }
    }
}
