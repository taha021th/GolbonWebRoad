using GolbonWebRoad.Application.Exceptions;
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
        private readonly IUnitOfWork _unitOfWork;
        public UpdateOrderStatusCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task Handle(UpdateOrderStatusCommand request, CancellationToken cancellationToken)
        {
            var order = await _unitOfWork.OrderRepository.GetByIdAsync(request.OrderId);
            if (order==null)
                throw new NotFoundException("سفارش با این شناسه یافت نشد.");
            order.OrderStatus=request.OrderStatus;
            await _unitOfWork.OrderRepository.UpdateAsync(order);
            await _unitOfWork.CompleteAsync();

        }
    }
}
