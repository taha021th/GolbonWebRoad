using AutoMapper;
using GolbonWebRoad.Application.Dtos.Orders;
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
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        public GetOrdersByUserIdQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork= unitOfWork;
            _mapper = mapper;

        }

        public async Task<IEnumerable<OrderDto>> Handle(GetOrdersByUserIdQuery request, CancellationToken cancellationToken)
        {
            var orders = await _unitOfWork.OrderRepository.GetByUserIdAsync(request.UserId);
            return _mapper.Map<IEnumerable<OrderDto>>(orders);
        }
    }
}
