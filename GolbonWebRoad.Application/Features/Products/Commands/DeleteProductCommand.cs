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
        private readonly IProductRepository _productRepository;

        public DeleteProductCommandHandler(IProductRepository productRepository)
        {
            _productRepository = productRepository;
        }

        public async Task Handle(DeleteProductCommand request, CancellationToken cancellationToken)
        {
            await _productRepository.DeleteAsync(request.Id);
        }
    }
}
