using FluentValidation;
using GolbonWebRoad.Domain.Entities;
using GolbonWebRoad.Domain.Interfaces;
using MediatR;

namespace GolbonWebRoad.Application.Features.Products.ProductVariants.Queries
{
    public class GetByIdProductVariantQuery : IRequest<ProductVariant>
    {
        public int Id { get; set; }
    }

    public class GetByIdProductVariantQueryValidator : AbstractValidator<GetByIdProductVariantQuery>
    {
        public GetByIdProductVariantQueryValidator()
        {
            RuleFor(x => x.Id).GreaterThan(0);
        }
    }

    public class GetByIdProductVariantQueryHandler : IRequestHandler<GetByIdProductVariantQuery, ProductVariant>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetByIdProductVariantQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<ProductVariant> Handle(GetByIdProductVariantQuery request, CancellationToken cancellationToken)
        {
            return await _unitOfWork.ProductVariantRepository.GetByIdAsync(request.Id);
        }
    }
}


