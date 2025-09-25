using FluentValidation;
using GolbonWebRoad.Domain.Entities;
using GolbonWebRoad.Domain.Interfaces;
using MediatR;

namespace GolbonWebRoad.Application.Features.Products.ProductAttributes.Queries
{
    public class GetByIdProductAttributeQuery : IRequest<ProductAttribute>
    {
        public int Id { get; set; }
    }

    public class GetByIdProductAttributeQueryValidator : AbstractValidator<GetByIdProductAttributeQuery>
    {
        public GetByIdProductAttributeQueryValidator()
        {
            RuleFor(x => x.Id).GreaterThan(0);
        }
    }

    public class GetByIdProductAttributeQueryHandler : IRequestHandler<GetByIdProductAttributeQuery, ProductAttribute>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetByIdProductAttributeQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<ProductAttribute> Handle(GetByIdProductAttributeQuery request, CancellationToken cancellationToken)
        {
            return await _unitOfWork.ProductAttributeRepository.GetByIdAsync(request.Id);
        }
    }
}
