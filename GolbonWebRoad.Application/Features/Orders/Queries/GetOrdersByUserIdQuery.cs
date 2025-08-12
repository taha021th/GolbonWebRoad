using AutoMapper;
using GolbonWebRoad.Application.Dtos;
using GolbonWebRoad.Domain.Interfaces;
using MediatR;

namespace GolbonWebRoad.Application.Features.Orders.Queries
{
    public class GetOrdersByUserIdQuery : IRequest<IEnumerable<OrderDto>>
    {
        public string UserId { get; set; }
    }
    public class GetOrdersByUserIdQueryHandler : IRequestHandler<GetOrdersByUserIdQuery, IEnumerable<OrderDto>>
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IMapper _mapper;
        public GetOrdersByUserIdQueryHandler(IOrderRepository orderRepository, IMapper mapper)
        {
            _orderRepository = orderRepository;
            _mapper = mapper;

        }

        public async Task<IEnumerable<OrderDto>> Handle(GetOrdersByUserIdQuery request, CancellationToken cancellationToken)
        {
            var orders = await _orderRepository.GetByUserIdAsync(request.UserId);
            return _mapper.Map<IEnumerable<OrderDto>>(orders);
        }
    }
}
