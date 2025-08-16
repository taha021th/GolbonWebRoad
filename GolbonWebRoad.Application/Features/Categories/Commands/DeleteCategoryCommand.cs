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
        private readonly ICategoryRepository _categoryRepository;
        private readonly IMapper _mapper;
        public DeleteCategoryCommandHandler(ICategoryRepository categoryRepository, IMapper mapper)
        {
            _categoryRepository=categoryRepository;
            _mapper=mapper;
        }
        public async Task Handle(DeleteCategoryCommand request, CancellationToken cancellationToken)
        {
            await _categoryRepository.DeleteAsync(request.Id);

        }
    }


}
