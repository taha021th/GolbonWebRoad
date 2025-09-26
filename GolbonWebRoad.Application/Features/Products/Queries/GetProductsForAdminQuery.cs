using AutoMapper;
using GolbonWebRoad.Domain.Entities;
using GolbonWebRoad.Domain.Interfaces;
using MediatR;

namespace GolbonWebRoad.Application.Features.Products.Queries
{
    public class GetProductsForAdminQuery : IRequest<IEnumerable<Product>>
    {
        public bool? JoinCategory { get; set; }
        public bool? JoinReviews { get; set; }
        public bool? JoinImages { get; set; }
        public bool? JoinBrand { get; set; }
        public bool? JoinColors { get; set; }
    }
    public class GetProductsForAdminQueryHandler : IRequestHandler<GetProductsForAdminQuery, IEnumerable<Product>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        public GetProductsForAdminQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork=unitOfWork;
            _mapper=mapper;
        }
        public async Task<IEnumerable<Product>> Handle(GetProductsForAdminQuery request, CancellationToken cancellationToken)
        {
            var products = await _unitOfWork.ProductRepository.GetAllAsync(joinCategory: request.JoinCategory, joinImages: request.JoinImages, joinReviews: request.JoinReviews, joinBrand: request.JoinBrand);

            return products;
        }
    }
}
