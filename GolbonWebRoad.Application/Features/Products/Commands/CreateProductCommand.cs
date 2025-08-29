using AutoMapper;
using FluentValidation;
using GolbonWebRoad.Application.Dtos.Products;
using GolbonWebRoad.Domain.Entities;
using GolbonWebRoad.Domain.Interfaces;
using MediatR;

namespace GolbonWebRoad.Application.Features.Products.Commands
{
    public class CreateProductCommand : IRequest<ProductDto>
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public string ImageUrl { get; set; }
        public int CategoryId { get; set; }
    }

    public class CreateProductCommandValidator : AbstractValidator<CreateProductCommand>
    {
        public CreateProductCommandValidator()
        {
            RuleFor(p => p.Name)
                .NotEmpty().WithMessage("نام محصول نمی تواند خالی باشد.")
                .MaximumLength(100).WithMessage("نام محصول نمی تواند بیشتر از 100 کاراکتر باشد.");
            RuleFor(p => p.Price)
                .GreaterThan(0).WithMessage("قیمت محصول باید بیشتر از صفر باشد.");
        }
    }
    public class CreateProductCommandHandler : IRequestHandler<CreateProductCommand, ProductDto>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        public CreateProductCommandHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper=mapper;

        }

        public async Task<ProductDto> Handle(CreateProductCommand request, CancellationToken cancellationToken)
        {
            var product = _mapper.Map<Product>(request);
            var newProduct = _unitOfWork.ProductRepository.Add(product);
            await _unitOfWork.CompleteAsync();

            return _mapper.Map<ProductDto>(newProduct);
        }
    }
}
