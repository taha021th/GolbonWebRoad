using FluentValidation;
using GolbonWebRoad.Domain.Entities;
using GolbonWebRoad.Domain.Interfaces;
using GolbonWebRoad.Domain.Interfaces.Repositories;
using MediatR;

namespace GolbonWebRoad.Application.Features.Products.ProductVariants.Queries
{
    public class GetAllProductVariantsQuery : IRequest<PagedResult<ProductVariant>>
    {
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 20;
    }

    public class GetAllProductVariantsQueryValidator : AbstractValidator<GetAllProductVariantsQuery>
    {
        public GetAllProductVariantsQueryValidator()
        {
            RuleFor(x => x.PageNumber).GreaterThan(0);
            RuleFor(x => x.PageSize).GreaterThan(0).LessThanOrEqualTo(200);
        }
    }

    public class GetAllProductVariantsQueryHandler : IRequestHandler<GetAllProductVariantsQuery, PagedResult<ProductVariant>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetAllProductVariantsQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<PagedResult<ProductVariant>> Handle(GetAllProductVariantsQuery request, CancellationToken cancellationToken)
        {
            return await _unitOfWork.ProductVariantRepository.GetAllAsync(request.PageNumber, request.PageSize);
        }
    }
}


