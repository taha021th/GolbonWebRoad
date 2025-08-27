using AutoMapper;
using FluentValidation;
using GolbonWebRoad.Domain.Interfaces;
using MediatR;

namespace GolbonWebRoad.Application.Features.Categories.Commands
{
    public class DeleteCategoryCommand : IRequest
    {
        public int Id { get; set; }
    }
    public class DeleteCategoryCommandValidator : AbstractValidator<DeleteCategoryCommand>
    {
        public DeleteCategoryCommandValidator()
        {
            RuleFor(c => c.Id).NotEmpty().WithMessage("شناسه دسته بندی نمی تواند خالی باشد.");
        }
    }
    public class DeleteCategoryCommandHandler : IRequestHandler<DeleteCategoryCommand>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        public DeleteCategoryCommandHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork=unitOfWork;
            _mapper=mapper;
        }
        public async Task Handle(DeleteCategoryCommand request, CancellationToken cancellationToken)
        {
            await _unitOfWork.CategoryRepository.DeleteAsync(request.Id);
            await _unitOfWork.CompleteAsync();

        }
    }


}
