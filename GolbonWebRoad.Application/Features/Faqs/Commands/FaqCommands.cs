using AutoMapper;
using GolbonWebRoad.Domain.Entities;
using MediatR;
using GolbonWebRoad.Domain.Interfaces;

namespace GolbonWebRoad.Application.Features.Faqs.Commands
{
    public class CreateFaqCommand : IRequest<int>
    {
        public string? Slog { get; set; }
        public string Question { get; set; } = default!;
        public string Answer { get; set; } = default!;
        public int? FaqCategoryId { get; set; }
        public string? Tags { get; set; }
        public int SortOrder { get; set; }
        public bool IsActive { get; set; } = true;
    }

    public class UpdateFaqCommand : IRequest
    {
        public int Id { get; set; }
        public string? Slog { get; set; }
        public string Question { get; set; } = default!;
        public string Answer { get; set; } = default!;
        public int? FaqCategoryId { get; set; }
        public string? Tags { get; set; }
        public int SortOrder { get; set; }
        public bool IsActive { get; set; } = true;
    }

    public class DeleteFaqCommand : IRequest
    {
        public int Id { get; set; }
    }

    public class CreateFaqCommandHandler : IRequestHandler<CreateFaqCommand, int>
    {
        private readonly IUnitOfWork _uow;
        private readonly IMapper _mapper;
        public CreateFaqCommandHandler(IUnitOfWork uow, IMapper mapper)
        {
            _uow = uow; _mapper = mapper;
        }
        public async Task<int> Handle(CreateFaqCommand request, CancellationToken cancellationToken)
        {
            var entity = _mapper.Map<Faq>(request);
            await _uow.FaqRepository.AddAsync(entity);
            await _uow.CompleteAsync();
            return entity.Id;
        }
    }

    public class UpdateFaqCommandHandler : IRequestHandler<UpdateFaqCommand>
    {
        private readonly IUnitOfWork _uow;
        private readonly IMapper _mapper;
        public UpdateFaqCommandHandler(IUnitOfWork uow, IMapper mapper)
        {
            _uow = uow; _mapper = mapper;
        }
        public async Task Handle(UpdateFaqCommand request, CancellationToken cancellationToken)
        {
            var existing = await _uow.FaqRepository.GetByIdAsync(request.Id);
            if (existing == null) throw new GolbonWebRoad.Application.Exceptions.NotFoundException("FAQ یافت نشد");
            _mapper.Map(request, existing);
            _uow.FaqRepository.Update(existing);
            await _uow.CompleteAsync();
        }
    }

    public class DeleteFaqCommandHandler : IRequestHandler<DeleteFaqCommand>
    {
        private readonly IUnitOfWork _uow;
        public DeleteFaqCommandHandler(IUnitOfWork uow) { _uow = uow; }
        public async Task Handle(DeleteFaqCommand request, CancellationToken cancellationToken)
        {
            await _uow.FaqRepository.DeleteAsync(request.Id);
            await _uow.CompleteAsync();
        }
    }
}
