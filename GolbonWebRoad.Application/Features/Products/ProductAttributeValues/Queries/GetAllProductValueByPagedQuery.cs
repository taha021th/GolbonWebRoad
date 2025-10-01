using GolbonWebRoad.Domain.Entities;
using GolbonWebRoad.Domain.Interfaces;
using GolbonWebRoad.Domain.Interfaces.Repositories;
using MediatR;

namespace GolbonWebRoad.Application.Features.Products.ProductAttributeValues.Queries
{
    public class GetAllProductValueByPagedQuery : IRequest<PagedResult<ProductAttributeValue>>
    {

        public int PageSize { get; set; }
        public int PageNumber { get; set; }

    }
    public class GetAllProductValueByPagedQueryHandler : IRequestHandler<GetAllProductValueByPagedQuery, PagedResult<ProductAttributeValue>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetAllProductValueByPagedQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork=unitOfWork;
        }
        public async Task<PagedResult<ProductAttributeValue>> Handle(GetAllProductValueByPagedQuery request, CancellationToken cancellationToken)
        {
            return await _unitOfWork.ProductAttributeValueRepository.GetAllByPagedAsync(request.PageNumber, request.PageSize);
        }
    }
}
