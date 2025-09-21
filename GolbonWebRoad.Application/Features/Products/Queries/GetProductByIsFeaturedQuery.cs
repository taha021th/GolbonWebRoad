using GolbonWebRoad.Domain.Entities;
using GolbonWebRoad.Domain.Interfaces;
using MediatR;

namespace GolbonWebRoad.Application.Features.Products.Queries
{
    public class GetProductByIsFeaturedQuery : IRequest<Product>
    {
    }
    public class GetProductByIsFeaturedQueryHandler : IRequestHandler<GetProductByIsFeaturedQuery, Product>
    {
        private readonly IUnitOfWork _unitOfWork;
        public GetProductByIsFeaturedQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;

        }

        public async Task<Product> Handle(GetProductByIsFeaturedQuery request, CancellationToken cancellationToken)
        {
            return await _unitOfWork.ProductRepository.GetProductByIsFeaturedAsync();
        }
    }
}
