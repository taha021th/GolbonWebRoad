using FluentValidation;
using GolbonWebRoad.Domain.Interfaces;
using MediatR;

namespace GolbonWebRoad.Application.Features.Products.Commands
{
    public class DeleteProductCommand : IRequest
    {
        public int Id { get; set; }
    }

    public class DeleteProductCommandValidator : AbstractValidator<DeleteProductCommand>
    {
        public DeleteProductCommandValidator()
        {
            RuleFor(p => p.Id).NotEmpty().WithMessage("Product ID cannot be empty.");
        }
    }
    public class DeleteProductCommandHandler : IRequestHandler<DeleteProductCommand>
    {
        private readonly IUnitOfWork _unitOfWork;

        public DeleteProductCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork=unitOfWork;
        }

        public async Task Handle(DeleteProductCommand request, CancellationToken cancellationToken)
        {
            await _unitOfWork.ProductRepository.DeleteAsync(request.Id);
            await _unitOfWork.CompleteAsync();
        }
    }
}
