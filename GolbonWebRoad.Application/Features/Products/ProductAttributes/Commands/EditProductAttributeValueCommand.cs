using FluentValidation;
using GolbonWebRoad.Application.Exceptions;
using GolbonWebRoad.Domain.Interfaces;
using MediatR;

namespace GolbonWebRoad.Application.Features.Products.ProductAttributes.Commands
{
    public class EditProductAttributeValueCommand : IRequest
    {
        public int Id { get; set; }
        public int AttributeId { get; set; }
        public string Value { get; set; }
    }

    public class EditProductAttributeValueCommandValidator : AbstractValidator<EditProductAttributeValueCommand>
    {
        public EditProductAttributeValueCommandValidator()
        {
            RuleFor(x => x.Id).GreaterThan(0);
            RuleFor(x => x.AttributeId).GreaterThan(0);
            RuleFor(x => x.Value).NotEmpty().MaximumLength(100);
        }
    }

    public class EditProductAttributeValueCommandHandler : IRequestHandler<EditProductAttributeValueCommand>
    {
        private readonly IUnitOfWork _unitOfWork;

        public EditProductAttributeValueCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task Handle(EditProductAttributeValueCommand request, CancellationToken cancellationToken)
        {
            var entity = await _unitOfWork.ProductAttributeValueRepository.GetByIdAsync(request.Id);
            if (entity == null)
            {
                throw new NotFoundException($"مقدار ویژگی با شناسه {request.Id} یافت نشد");
            }

            entity.AttributeId = request.AttributeId;
            entity.Value = request.Value.Trim();

            _unitOfWork.ProductAttributeValueRepository.Update(entity);
            await _unitOfWork.CompleteAsync();
        }
    }
}


