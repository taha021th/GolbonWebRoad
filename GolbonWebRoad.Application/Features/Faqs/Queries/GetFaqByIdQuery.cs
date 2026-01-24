using AutoMapper;
using GolbonWebRoad.Application.Dtos.Faqs;
using GolbonWebRoad.Application.Exceptions;
using GolbonWebRoad.Domain.Interfaces;
using MediatR;

namespace GolbonWebRoad.Application.Features.Faqs.Queries
{
    public class GetFaqByIdQuery : IRequest<FaqDto>
    {
        public int Id { get; set; }
    }

    public class GetFaqByIdQueryHandler : IRequestHandler<GetFaqByIdQuery, FaqDto>
    {
        private readonly IUnitOfWork _uow;
        private readonly IMapper _mapper;
        public GetFaqByIdQueryHandler(IUnitOfWork uow, IMapper mapper) { _uow = uow; _mapper = mapper; }
        public async Task<FaqDto> Handle(GetFaqByIdQuery request, CancellationToken cancellationToken)
        {
            var entity = await _uow.FaqRepository.GetByIdAsync(request.Id);
            if (entity == null) throw new NotFoundException("FAQ یافت نشد");
            return _mapper.Map<FaqDto>(entity);
        }
    }
}
