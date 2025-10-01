using FluentValidation;
using GolbonWebRoad.Domain.Entities;
using GolbonWebRoad.Domain.Interfaces;
using GolbonWebRoad.Domain.Interfaces.Repositories;
using MediatR;

namespace GolbonWebRoad.Application.Features.Products.ProductAttributes.queries
{
    public class GetAllProductAttributeByPagedQuery : IRequest<PagedResult<ProductAttribute>>
    {
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 20;
    }

    public class GetAllProductAttributeQueryValidator : AbstractValidator<GetAllProductAttributeByPagedQuery>
    {
        public GetAllProductAttributeQueryValidator()
        {
            RuleFor(x => x.PageNumber).GreaterThan(0);
            RuleFor(x => x.PageSize).GreaterThan(0).LessThanOrEqualTo(200);
        }
    }

    public class GetAllProductAttributeQueryHandler : IRequestHandler<GetAllProductAttributeByPagedQuery, PagedResult<ProductAttribute>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetAllProductAttributeQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<PagedResult<ProductAttribute>> Handle(GetAllProductAttributeByPagedQuery request, CancellationToken cancellationToken)
        {
            return await _unitOfWork.ProductAttributeRepository.GetAllByPagedAsync(request.PageNumber, request.PageSize);
        }
    }
}
