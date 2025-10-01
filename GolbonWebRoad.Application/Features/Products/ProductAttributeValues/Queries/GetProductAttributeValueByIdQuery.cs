using GolbonWebRoad.Domain.Entities;
using GolbonWebRoad.Domain.Interfaces;
using MediatR;

namespace GolbonWebRoad.Application.Features.Products.ProductAttributeValues.Queries
{
    public class GetProductAttributeValueByIdQuery : IRequest<ProductAttributeValue>
    {
        public int Id { get; set; }
    }

    public class GetProductAttributeValueByIdQueryHandler : IRequestHandler<GetProductAttributeValueByIdQuery, ProductAttributeValue>
    {
        private readonly IUnitOfWork _unitOfWork;
        public GetProductAttributeValueByIdQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task<ProductAttributeValue> Handle(GetProductAttributeValueByIdQuery request, CancellationToken cancellationToken)
        {
            return await _unitOfWork.ProductAttributeValueRepository.GetByIdAsync(request.Id);
        }
    }
}