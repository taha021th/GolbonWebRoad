using FluentValidation;
using GolbonWebRoad.Domain.Interfaces;
using MediatR;

namespace GolbonWebRoad.Application.Features.Products.ProductVariants.Commands
{
    public class DeleteProductVariantCommand : IRequest
    {
        public int Id { get; set; }
    }

    public class DeleteProductVariantCommandValidator : AbstractValidator<DeleteProductVariantCommand>
    {
        public DeleteProductVariantCommandValidator()
        {
            RuleFor(x => x.Id).GreaterThan(0);
        }
    }

    public class DeleteProductVariantCommandHandler : IRequestHandler<DeleteProductVariantCommand>
    {
        private readonly IUnitOfWork _unitOfWork;

        public DeleteProductVariantCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task Handle(DeleteProductVariantCommand request, CancellationToken cancellationToken)
        {
            await _unitOfWork.ProductVariantRepository.RemoveAsync(request.Id);
            await _unitOfWork.CompleteAsync();
        }
    }
}


