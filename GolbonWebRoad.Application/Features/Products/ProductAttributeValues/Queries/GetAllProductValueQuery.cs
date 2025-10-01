using GolbonWebRoad.Domain.Entities;
using GolbonWebRoad.Domain.Interfaces;
using MediatR;

namespace GolbonWebRoad.Application.Features.Products.ProductAttributeValues.Queries
{
    public class GetAllProductValueQuery : IRequest<IEnumerable<ProductAttributeValue>>
    {
    }
    public class GetAllProductValueQueryHandler : IRequestHandler<GetAllProductValueQuery, IEnumerable<ProductAttributeValue>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetAllProductValueQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork=unitOfWork;
        }
        public async Task<IEnumerable<ProductAttributeValue>> Handle(GetAllProductValueQuery request, CancellationToken cancellationToken)
        {
            return await _unitOfWork.ProductAttributeValueRepository.GetAllAsync();
        }
    }

}
