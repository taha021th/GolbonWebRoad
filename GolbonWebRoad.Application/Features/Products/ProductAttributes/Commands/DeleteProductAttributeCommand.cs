using FluentValidation;
using GolbonWebRoad.Domain.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace GolbonWebRoad.Application.Features.Products.ProductAttributes.Commands
{
    public class DeleteProductAttributeCommand : IRequest
    {
        public int Id { get; set; }
    }

    public class DeleteProductAttributeCommandValidator : AbstractValidator<DeleteProductAttributeCommand>
    {
        public DeleteProductAttributeCommandValidator()
        {
            RuleFor(x => x.Id).GreaterThan(0);
        }
    }

    public class DeleteProductAttributeCommandHandler : IRequestHandler<DeleteProductAttributeCommand>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<DeleteProductAttributeCommandHandler> _logger;

        public DeleteProductAttributeCommandHandler(IUnitOfWork unitOfWork, ILogger<DeleteProductAttributeCommandHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task Handle(DeleteProductAttributeCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("حذف ویژگی محصول {Id}", request.Id);
            await _unitOfWork.ProductAttributeRepository.RemoveAsync(request.Id);
            await _unitOfWork.CompleteAsync();
        }
    }
}
