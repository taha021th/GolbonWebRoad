using FluentValidation;
using GolbonWebRoad.Domain.Entities;
using GolbonWebRoad.Domain.Interfaces;
using MediatR;

namespace GolbonWebRoad.Application.Features.Products.ProductAttributes.Queries
{
    public class GetByIdProductAttributeValueQuery : IRequest<ProductAttributeValue>
    {
        public int Id { get; set; }
    }

    public class GetByIdProductAttributeValueQueryValidator : AbstractValidator<GetByIdProductAttributeValueQuery>
    {
        public GetByIdProductAttributeValueQueryValidator()
        {
            RuleFor(x => x.Id).GreaterThan(0);
        }
    }

    public class GetByIdProductAttributeValueQueryHandler : IRequestHandler<GetByIdProductAttributeValueQuery, ProductAttributeValue>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetByIdProductAttributeValueQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<ProductAttributeValue> Handle(GetByIdProductAttributeValueQuery request, CancellationToken cancellationToken)
        {
            return await _unitOfWork.ProductAttributeValueRepository.GetByIdAsync(request.Id);
        }
    }
}


