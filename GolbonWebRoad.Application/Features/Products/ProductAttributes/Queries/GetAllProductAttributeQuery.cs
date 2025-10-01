using GolbonWebRoad.Domain.Entities;
using GolbonWebRoad.Domain.Interfaces;
using MediatR;

namespace GolbonWebRoad.Application.Features.Products.ProductAttributes.Queries
{
    public class GetAllProductAttributeQuery : IRequest<ICollection<ProductAttribute>>
    {
    }
    public class GetAllProductAttributeQueryHandler : IRequestHandler<GetAllProductAttributeQuery, ICollection<ProductAttribute>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetAllProductAttributeQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork=unitOfWork;
        }

        public async Task<ICollection<ProductAttribute>> Handle(GetAllProductAttributeQuery request, CancellationToken cancellationToken)
        {
            return await _unitOfWork.ProductAttributeRepository.GetAllAsync();
        }
    }
}
