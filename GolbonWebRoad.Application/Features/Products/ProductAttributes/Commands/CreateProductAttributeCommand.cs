using FluentValidation;
using GolbonWebRoad.Domain.Entities;
using GolbonWebRoad.Domain.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace GolbonWebRoad.Application.Features.Products.ProductAttributes.Commands
{
    public class CreateProductAttributeCommand : IRequest
    {
        public string Name { get; set; }
    }

    public class CreateProductAttributeCommandValidator : AbstractValidator<CreateProductAttributeCommand>
    {
        public CreateProductAttributeCommandValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("نام ویژگی نمی‌تواند خالی باشد")
                .MaximumLength(100).WithMessage("حداکثر طول نام ویژگی 100 کاراکتر است");
        }
    }

    public class CreateProductAttributeCommandHandler : IRequestHandler<CreateProductAttributeCommand>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<CreateProductAttributeCommandHandler> _logger;

        public CreateProductAttributeCommandHandler(IUnitOfWork unitOfWork, ILogger<CreateProductAttributeCommandHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task Handle(CreateProductAttributeCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("ایجاد ویژگی محصول با نام {Name}", request.Name);

            var entity = new ProductAttribute
            {
                Name = request.Name.Trim()
            };

            _unitOfWork.ProductAttributeRepository.Add(entity);
            await _unitOfWork.CompleteAsync();

            _logger.LogInformation("ویژگی محصول با شناسه {Id} ایجاد شد", entity.Id);
        }
    }
}
