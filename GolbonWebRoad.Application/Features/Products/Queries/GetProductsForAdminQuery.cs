using AutoMapper;
using GolbonWebRoad.Application.Dtos.Products;
using GolbonWebRoad.Domain.Interfaces;
using MediatR;

namespace GolbonWebRoad.Application.Features.Products.Queries
{
    public class GetProductsForAdminQuery : IRequest<IEnumerable<ProductAdminSummaryDto>>
    {
        public bool? JoinCategory { get; set; }
        public bool? JoinReviews { get; set; }
        public bool? JoinImages { get; set; }
    }
    public class GetProductsForAdminQueryHandler : IRequestHandler<GetProductsForAdminQuery, IEnumerable<ProductAdminSummaryDto>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        public GetProductsForAdminQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork=unitOfWork;
            _mapper=mapper;
        }
        public async Task<IEnumerable<ProductAdminSummaryDto>> Handle(GetProductsForAdminQuery request, CancellationToken cancellationToken)
        {
            var products = await _unitOfWork.ProductRepository.GetAllAsync(joinCategory: request.JoinCategory, joinImages: request.JoinImages, joinReviews: request.JoinReviews);

            return _mapper.Map<IEnumerable<ProductAdminSummaryDto>>(products);
        }
    }
}
