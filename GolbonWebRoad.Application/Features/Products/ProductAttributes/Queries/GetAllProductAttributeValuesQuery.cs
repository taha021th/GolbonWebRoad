using FluentValidation;
using GolbonWebRoad.Domain.Entities;
using GolbonWebRoad.Domain.Interfaces.Repositories;
using GolbonWebRoad.Domain.Interfaces;
using MediatR;

namespace GolbonWebRoad.Application.Features.Products.ProductAttributes.Queries
{
    public class GetAllProductAttributeValuesQuery : IRequest<PagedResult<ProductAttributeValue>>
    {
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 20;
    }

    public class GetAllProductAttributeValuesQueryValidator : AbstractValidator<GetAllProductAttributeValuesQuery>
    {
        public GetAllProductAttributeValuesQueryValidator()
        {
            RuleFor(x => x.PageNumber).GreaterThan(0);
            RuleFor(x => x.PageSize).GreaterThan(0).LessThanOrEqualTo(200);
        }
    }

    public class GetAllProductAttributeValuesQueryHandler : IRequestHandler<GetAllProductAttributeValuesQuery, PagedResult<ProductAttributeValue>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetAllProductAttributeValuesQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<PagedResult<ProductAttributeValue>> Handle(GetAllProductAttributeValuesQuery request, CancellationToken cancellationToken)
        {
            return await _unitOfWork.ProductAttributeValueRepository.GetAllAsync(request.PageNumber, request.PageSize);
        }
    }
}


