using FluentValidation;
using GolbonWebRoad.Domain.Interfaces;
using MediatR;

namespace GolbonWebRoad.Application.Features.Products.ProductAttributes.Commands
{
    public class DeleteProductAttributeValueCommand : IRequest
    {
        public int Id { get; set; }
    }

    public class DeleteProductAttributeValueCommandValidator : AbstractValidator<DeleteProductAttributeValueCommand>
    {
        public DeleteProductAttributeValueCommandValidator()
        {
            RuleFor(x => x.Id).GreaterThan(0);
        }
    }

    public class DeleteProductAttributeValueCommandHandler : IRequestHandler<DeleteProductAttributeValueCommand>
    {
        private readonly IUnitOfWork _unitOfWork;

        public DeleteProductAttributeValueCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task Handle(DeleteProductAttributeValueCommand request, CancellationToken cancellationToken)
        {
            await _unitOfWork.ProductAttributeValueRepository.RemoveAsync(request.Id);
            await _unitOfWork.CompleteAsync();
        }
    }
}


