using GolbonWebRoad.Domain.Entities;
using GolbonWebRoad.Domain.Interfaces;
using MediatR;

namespace GolbonWebRoad.Application.Features.Faqs.Commands
{
    public class CreateFaqCategoryCommand : IRequest<int>
    {
        public string Name { get; set; } = default!;
        public string? Slog { get; set; }
        public int SortOrder { get; set; }
        public bool IsActive { get; set; } = true;
    }

    public class UpdateFaqCategoryCommand : IRequest
    {
        public int Id { get; set; }
        public string Name { get; set; } = default!;
        public string? Slog { get; set; }
        public int SortOrder { get; set; }
        public bool IsActive { get; set; } = true;
    }

    public class DeleteFaqCategoryCommand : IRequest
    {
        public int Id { get; set; }
    }

    public class CreateFaqCategoryCommandHandler : IRequestHandler<CreateFaqCategoryCommand, int>
    {
        private readonly IUnitOfWork _uow;
        public CreateFaqCategoryCommandHandler(IUnitOfWork uow) { _uow = uow; }
        public async Task<int> Handle(CreateFaqCategoryCommand request, CancellationToken cancellationToken)
        {
            var entity = new FaqCategory { Name = request.Name, Slog = request.Slog, SortOrder = request.SortOrder, IsActive = request.IsActive };
            await _uow.FaqCategoryRepository.AddAsync(entity);
            await _uow.CompleteAsync();
            return entity.Id;
        }
    }

    public class UpdateFaqCategoryCommandHandler : IRequestHandler<UpdateFaqCategoryCommand>
    {
        private readonly IUnitOfWork _uow;
        public UpdateFaqCategoryCommandHandler(IUnitOfWork uow) { _uow = uow; }
        public async Task Handle(UpdateFaqCategoryCommand request, CancellationToken cancellationToken)
        {
            var entity = await _uow.FaqCategoryRepository.GetByIdAsync(request.Id) ?? throw new GolbonWebRoad.Application.Exceptions.NotFoundException("دسته سوالات یافت نشد");
            entity.Name = request.Name;
            entity.Slog = request.Slog;
            entity.SortOrder = request.SortOrder;
            entity.IsActive = request.IsActive;
            _uow.FaqCategoryRepository.Update(entity);
            await _uow.CompleteAsync();
        }
    }

    public class DeleteFaqCategoryCommandHandler : IRequestHandler<DeleteFaqCategoryCommand>
    {
        private readonly IUnitOfWork _uow;
        public DeleteFaqCategoryCommandHandler(IUnitOfWork uow) { _uow = uow; }
        public async Task Handle(DeleteFaqCategoryCommand request, CancellationToken cancellationToken)
        {
            await _uow.FaqCategoryRepository.DeleteAsync(request.Id);
            await _uow.CompleteAsync();
        }
    }
}
