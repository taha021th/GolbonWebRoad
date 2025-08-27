using AutoMapper;
using GolbonWebRoad.Application.Dtos.Products;
using GolbonWebRoad.Domain.Entities;
using GolbonWebRoad.Domain.Interfaces;
using MediatR;

namespace GolbonWebRoad.Application.Features.Products.Queries
{
    public class GetProductsQuery : IRequest<IEnumerable<ProductDto>>
    {
        public string? SearchTerm { get; set; }
        public int? CategoryId { get; set; }
        public string? SortOrder { get; set; }
        public bool? JoinCategory { get; set; }
    }

    public class GetProductsQueryHandler : IRequestHandler<GetProductsQuery, IEnumerable<ProductDto>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        public GetProductsQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork=unitOfWork;
            _mapper= mapper;
        }

        public async Task<IEnumerable<ProductDto>> Handle(GetProductsQuery request, CancellationToken cancellationToken)
        {
            IEnumerable<Product> products = await _unitOfWork.ProductRepository.GetAllAsync(request.SearchTerm, request.CategoryId, request.SortOrder, request.JoinCategory);
            return _mapper.Map<IEnumerable<ProductDto>>(products);
        }
    }
}
