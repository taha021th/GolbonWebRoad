using FluentValidation;
using GolbonWebRoad.Application.Exceptions;
using GolbonWebRoad.Domain.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace GolbonWebRoad.Application.Features.Products.ProductAttributes.Commands
{
    public class EditProductAttributeCommand : IRequest
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }

    public class EditProductAttributeCommandValidator : AbstractValidator<EditProductAttributeCommand>
    {
        public EditProductAttributeCommandValidator()
        {
            RuleFor(x => x.Id).GreaterThan(0);
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("نام ویژگی نمی‌تواند خالی باشد")
                .MaximumLength(100).WithMessage("حداکثر طول نام ویژگی 100 کاراکتر است");
        }
    }

    public class EditProductAttributeCommandHandler : IRequestHandler<EditProductAttributeCommand>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<EditProductAttributeCommandHandler> _logger;

        public EditProductAttributeCommandHandler(IUnitOfWork unitOfWork, ILogger<EditProductAttributeCommandHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task Handle(EditProductAttributeCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("ویرایش ویژگی محصول {Id}", request.Id);

            var entity = await _unitOfWork.ProductAttributeRepository.GetByIdAsync(request.Id);
            if (entity == null)
            {
                throw new NotFoundException($"ویژگی با شناسه {request.Id} یافت نشد");
            }

            entity.Name = request.Name.Trim();

            _unitOfWork.ProductAttributeRepository.Update(entity);
            await _unitOfWork.CompleteAsync();

            _logger.LogInformation("ویژگی محصول {Id} با موفقیت ویرایش شد", entity.Id);
        }
    }
}
