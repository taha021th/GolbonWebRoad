using FluentValidation;
using GolbonWebRoad.Domain.Entities;
using GolbonWebRoad.Domain.Interfaces;
using MediatR;

namespace GolbonWebRoad.Application.Features.Products.ProductAttributes.Commands
{
    public class CreateProductAttributeValueCommand : IRequest
    {
        public int AttributeId { get; set; }
        public string Value { get; set; }
    }

    public class CreateProductAttributeValueCommandValidator : AbstractValidator<CreateProductAttributeValueCommand>
    {
        public CreateProductAttributeValueCommandValidator()
        {
            RuleFor(x => x.AttributeId).GreaterThan(0);
            RuleFor(x => x.Value).NotEmpty().MaximumLength(100);
        }
    }

    public class CreateProductAttributeValueCommandHandler : IRequestHandler<CreateProductAttributeValueCommand>
    {
        private readonly IUnitOfWork _unitOfWork;

        public CreateProductAttributeValueCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task Handle(CreateProductAttributeValueCommand request, CancellationToken cancellationToken)
        {
            var entity = new ProductAttributeValue
            {
                AttributeId = request.AttributeId,
                Value = request.Value.Trim()
            };

            _unitOfWork.ProductAttributeValueRepository.Add(entity);
            await _unitOfWork.CompleteAsync();
        }
    }
}


