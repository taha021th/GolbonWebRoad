using FluentValidation;
using GolbonWebRoad.Application.Exceptions;
using GolbonWebRoad.Domain.Interfaces;
using MediatR;

namespace GolbonWebRoad.Application.Features.Products.ProductVariants.Commands
{
    public class UpdateProductVariantCommand : IRequest
    {
        public int Id { get; set; }
        public string Sku { get; set; }
        public decimal Price { get; set; }
        public decimal? OldPrice { get; set; }
        public int StockQuantity { get; set; }
        public List<int> AttributeValueIds { get; set; } = new();
    }

    public class UpdateProductVariantCommandValidator : AbstractValidator<UpdateProductVariantCommand>
    {
        private readonly IUnitOfWork _unitOfWork;

        public UpdateProductVariantCommandValidator(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
            RuleFor(x => x.Id).GreaterThan(0);
            RuleFor(x => x.Sku).NotEmpty().MaximumLength(100);
            RuleFor(x => x.Price).GreaterThan(0);
            RuleFor(x => x.StockQuantity).GreaterThanOrEqualTo(0);

            RuleFor(x => x.AttributeValueIds)
                .Must(ids => ids == null || ids.Distinct().Count() == ids.Count)
                .WithMessage("مقادیر ویژگی تکراری انتخاب شده است")
                .MustAsync(async (cmd, ids, ct) =>
                {
                    if (ids == null || ids.Count == 0) return true;
                    var values = new List<Domain.Entities.ProductAttributeValue>();
                    foreach (var id in ids.Distinct())
                    {
                        var v = await _unitOfWork.ProductAttributeValueRepository.GetByIdAsync(id);
                        if (v == null) return false;
                        values.Add(v);
                    }
                    var distinctAttributeCount = values.Select(v => v.AttributeId).Distinct().Count();
                    return distinctAttributeCount == values.Count;
                })
                .WithMessage("برای هر ویژگی فقط یک مقدار می‌توان انتخاب کرد یا مقدار نامعتبر انتخاب شده است");
        }
    }

    public class UpdateProductVariantCommandHandler : IRequestHandler<UpdateProductVariantCommand>
    {
        private readonly IUnitOfWork _unitOfWork;

        public UpdateProductVariantCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task Handle(UpdateProductVariantCommand request, CancellationToken cancellationToken)
        {
            var variant = await _unitOfWork.ProductVariantRepository.GetByIdAsync(request.Id);
            if (variant == null)
            {
                throw new NotFoundException($"متغیر محصول با شناسه {request.Id} یافت نشد");
            }

            variant.Sku = request.Sku.Trim();
            variant.Price = request.Price;
            variant.OldPrice = request.OldPrice;
            variant.StockQuantity = request.StockQuantity;

            // به‌روزرسانی ویژگی‌های انتخاب‌شده: حذف همه و اضافه‌کردن جدیدها (ساده)
            variant.SelectedAttributes.Clear();
            if (request.AttributeValueIds != null && request.AttributeValueIds.Any())
            {
                foreach (var valueId in request.AttributeValueIds.Distinct())
                {
                    var value = await _unitOfWork.ProductAttributeValueRepository.GetByIdAsync(valueId);
                    if (value != null)
                    {
                        variant.SelectedAttributes.Add(value);
                    }
                }
            }

            _unitOfWork.ProductVariantRepository.Update(variant);
            await _unitOfWork.CompleteAsync();
        }
    }
}


