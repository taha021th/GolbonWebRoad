using AutoMapper;
using FluentValidation;
using GolbonWebRoad.Domain.Entities;
using GolbonWebRoad.Domain.Interfaces;
using MediatR;

namespace GolbonWebRoad.Application.Features.Products.ProductAttributeValues.Commands
{
    public class EditProductValueCommand : IRequest
    {
        public int Id { get; set; }
        public string Value { get; set; }
        public int AttributeId { get; set; }
    }

    public class EditProductValueCommandValidator : AbstractValidator<EditProductValueCommand>
    {
        public EditProductValueCommandValidator()
        {
            RuleFor(x => x.Id).GreaterThan(0);
            RuleFor(x => x.AttributeId).GreaterThan(0).WithMessage("انتخاب ویژگی الزامی است");
            RuleFor(x => x.Value)
                .NotEmpty().WithMessage("مقدار ویژگی نمی‌تواند خالی باشد")
                .MaximumLength(100).WithMessage("حداکثر طول مقدار 100 کاراکتر است");
        }
    }

    public class EditProductValueCommandHandler : IRequestHandler<EditProductValueCommand>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public EditProductValueCommandHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork=unitOfWork;
            _mapper=mapper;
        }
        public async Task Handle(EditProductValueCommand request, CancellationToken cancellationToken)
        {
            var entity = _mapper.Map<ProductAttributeValue>(request);
            _unitOfWork.ProductAttributeValueRepository.Update(entity);
            await _unitOfWork.CompleteAsync();
        }
    }
}
