using GolbonWebRoad.Domain.Entities;
using GolbonWebRoad.Domain.Interfaces;
using GolbonWebRoad.Domain.Interfaces.Repositories;
using MediatR;

namespace GolbonWebRoad.Application.Features.Products.ProductAttributeValues.Queries
{
    public class GetProductValuesByAttributeIdPagedQuery : IRequest<PagedResult<ProductAttributeValue>>
    {
        public int AttributeId { get; set; }
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 20;
    }

    public class GetProductValuesByAttributeIdPagedQueryHandler : IRequestHandler<GetProductValuesByAttributeIdPagedQuery, PagedResult<ProductAttributeValue>>
    {
        private readonly IUnitOfWork _unitOfWork;
        public GetProductValuesByAttributeIdPagedQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task<PagedResult<ProductAttributeValue>> Handle(GetProductValuesByAttributeIdPagedQuery request, CancellationToken cancellationToken)
        {
            return await _unitOfWork.ProductAttributeValueRepository.GetAllByAttributeIdAsync(request.AttributeId, request.PageNumber, request.PageSize);
        }
    }
}