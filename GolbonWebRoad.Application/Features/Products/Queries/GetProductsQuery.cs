using AutoMapper;
using GolbonWebRoad.Application.Dtos;
using GolbonWebRoad.Domain.Interfaces;
using MediatR;

namespace GolbonWebRoad.Application.Features.Products.Queries
{
    public class GetProductsQuery : IRequest<IEnumerable<ProductDto>>
    {
    }

    public class GetProductsQueryHandler : IRequestHandler<GetProductsQuery, IEnumerable<ProductDto>>
    {
        private readonly IProductRepository _productRepository;
        private readonly IMapper _mapper;
        public GetProductsQueryHandler(IProductRepository productRepository, IMapper mapper)
        {
            _productRepository= productRepository;
            _mapper= mapper;
        }

        public async Task<IEnumerable<ProductDto>> Handle(GetProductsQuery request, CancellationToken cancellationToken)
        {
            var products = await _productRepository.GetAllAsync();
            return _mapper.Map<IEnumerable<ProductDto>>(products);
        }
    }
}
