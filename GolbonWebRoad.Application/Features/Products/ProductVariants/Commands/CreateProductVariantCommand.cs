using FluentValidation;
using GolbonWebRoad.Domain.Interfaces;
using MediatR;

namespace GolbonWebRoad.Application.Features.Products.ProductVariants.Commands
{
    public class CreateProductVariantCommand : IRequest
    {
        public int ProductId { get; set; }
        public string Sku { get; set; }
        public decimal Price { get; set; }
        public decimal? OldPrice { get; set; }
        public int StockQuantity { get; set; }
        public List<int> AttributeValueIds { get; set; } = new();
    }

    public class CreateProductVariantCommandValidator : AbstractValidator<CreateProductVariantCommand>
    {
        private readonly IUnitOfWork _unitOfWork;

        public CreateProductVariantCommandValidator(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
            RuleFor(x => x.ProductId).GreaterThan(0);
            RuleFor(x => x.Sku).NotEmpty().MaximumLength(100);
            RuleFor(x => x.Price).GreaterThan(0);
            RuleFor(x => x.StockQuantity).GreaterThanOrEqualTo(0);
            RuleFor(x => x.OldPrice)
                .GreaterThan(x => x.Price)
                .When(x => x.OldPrice.HasValue)
                .WithMessage("قیمت قدیم باید بیشتر از قیمت باشد");

            RuleFor(x => x.AttributeValueIds)
                .Must(ids => ids == null || ids.Distinct().Count() == ids.Count)
                .WithMessage("مقادیر ویژگی تکراری انتخاب شده است")
                .MustAsync(async (cmd, ids, ct) =>
                {
                    if (ids == null || ids.Count == 0) return true; // انتخاب مقادیر ویژگی اختیاری است
                    var values = new List<Domain.Entities.ProductAttributeValue>();
                    foreach (var id in ids.Distinct())
                    {
                        var v = await _unitOfWork.ProductAttributeValueRepository.GetByIdAsync(id);
                        if (v == null) return false;
                        values.Add(v);
                    }
                    var distinctAttributeCount = values.Select(v => v.AttributeId).Distinct().Count();
                    return distinctAttributeCount == values.Count; // در هر ورینت، از هر ویژگی فقط یک مقدار
                })
                .WithMessage("برای هر ویژگی فقط یک مقدار می‌توان انتخاب کرد یا مقدار نامعتبر انتخاب شده است");
        }
    }

    public class CreateProductVariantCommandHandler : IRequestHandler<CreateProductVariantCommand>
    {
        private readonly IUnitOfWork _unitOfWork;

        public CreateProductVariantCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task Handle(CreateProductVariantCommand request, CancellationToken cancellationToken)
        {
            var variant = new ProductVariant
            {
                ProductId = request.ProductId,
                Sku = request.Sku.Trim(),
                Price = request.Price,
                OldPrice = request.OldPrice,
                StockQuantity = request.StockQuantity
            };

            if (request.AttributeValueIds != null && request.AttributeValueIds.Any())
            {
                foreach (var valueId in request.AttributeValueIds.Distinct())
                {
                    var value = await _unitOfWork.ProductAttributeValueRepository.GetByIdAsync(valueId);
                    if (value != null)
                    {
                        variant.AttributeValues.Add(value);
                    }
                }
            }

            _unitOfWork.ProductVariantRepository.Add(variant);
            await _unitOfWork.CompleteAsync();
        }
    }
}
