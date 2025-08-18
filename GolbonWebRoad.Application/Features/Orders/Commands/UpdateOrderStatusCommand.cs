using GolbonWebRoad.Domain.Interfaces;
using MediatR;

namespace GolbonWebRoad.Application.Features.Orders.Commands
{
    public class UpdateOrderStatusCommand : IRequest
    {
        public int OrderId { get; set; }
        public string OrderStatus { get; set; }
    }
    public class UpdateOrderStatusCommandHandler : IRequestHandler<UpdateOrderStatusCommand>
    {
        private readonly IOrderRepository _orderRepository;
        public UpdateOrderStatusCommandHandler(IOrderRepository orderRepository)
        {
            _orderRepository=orderRepository;
        }
        public async Task Handle(UpdateOrderStatusCommand request, CancellationToken cancellationToken)
        {
            var order = await _orderRepository.GetByIdAsync(request.OrderId);
            if (order !=null)
            {
                order.OrderStatus=request.OrderStatus;
                await _orderRepository.UpdateAsync(order);
            }
        }
    }
}
