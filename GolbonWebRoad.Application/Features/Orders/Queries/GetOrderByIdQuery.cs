using AutoMapper;
using GolbonWebRoad.Application.Dtos;
using GolbonWebRoad.Domain.Interfaces;
using MediatR;

namespace GolbonWebRoad.Application.Features.Orders.Queries
{
    public class GetOrderByIdQuery : IRequest<OrderDto>
    {
        public int Id { get; set; }
    }
    public class GetOrderByIdQueryHandler : IRequestHandler<GetOrderByIdQuery, OrderDto>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        public GetOrderByIdQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork=unitOfWork;
            _mapper = mapper;
        }
        public async Task<OrderDto> Handle(GetOrderByIdQuery request, CancellationToken cancellationToken)
        {
            var order = await _unitOfWork.OrderRepository.GetByIdAsync(request.Id);
            return _mapper.Map<OrderDto>(order);
        }
    }
}
