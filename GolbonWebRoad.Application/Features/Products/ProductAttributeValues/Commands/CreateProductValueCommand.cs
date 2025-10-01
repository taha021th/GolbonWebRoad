using AutoMapper;
using FluentValidation;
using GolbonWebRoad.Domain.Entities;
using GolbonWebRoad.Domain.Interfaces;
using MediatR;

namespace GolbonWebRoad.Application.Features.Products.ProductAttributeValues.Commands
{
    public class CreateProductValueCommand : IRequest
    {
        public string Value { get; set; }
        public int AttributeId { get; set; }
    }

    public class CreateProductValueCommandValidator : AbstractValidator<CreateProductValueCommand>
    {
        public CreateProductValueCommandValidator()
        {
            RuleFor(x => x.AttributeId).GreaterThan(0).WithMessage("انتخاب ویژگی الزامی است");
            RuleFor(x => x.Value)
                .NotEmpty().WithMessage("مقدار ویژگی نمی‌تواند خالی باشد")
                .MaximumLength(100).WithMessage("حداکثر طول مقدار 100 کاراکتر است");
        }
    }

    public class CreateProductValueCommandHandler : IRequestHandler<CreateProductValueCommand>
    {
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;

        public CreateProductValueCommandHandler(IMapper mapper, IUnitOfWork unitOfWork)
        {
            _mapper=mapper;
            _unitOfWork=unitOfWork;
        }
        public async Task Handle(CreateProductValueCommand request, CancellationToken cancellationToken)
        {
            var entity = _mapper.Map<ProductAttributeValue>(request);
            _unitOfWork.ProductAttributeValueRepository.Add(entity);
            await _unitOfWork.CompleteAsync();

        }
    }
}
