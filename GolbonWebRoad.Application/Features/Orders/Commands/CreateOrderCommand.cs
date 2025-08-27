using GolbonWebRoad.Application.Dtos;
using GolbonWebRoad.Domain.Entities;
using GolbonWebRoad.Domain.Interfaces;
using MediatR;

namespace GolbonWebRoad.Application.Features.Orders.Commands
{
    public class CreateOrderCommand : IRequest<int>
    {
        public string UserId { get; set; }
        public List<CartItemDto> CartItems { get; set; }
    }
    public class CreateOrderCommandHandler : IRequestHandler<CreateOrderCommand, int>
    {
        private readonly IUnitOfWork _unitOfWork;
        public CreateOrderCommandHandler(IUnitOfWork unitOfWork)
        {

            _unitOfWork = unitOfWork;
        }

        public async Task<int> Handle(CreateOrderCommand request, CancellationToken cancellationToken)
        {
            var order = new Order
            {
                UserId=request.UserId,
                OrderDate=DateTime.UtcNow,
                OrderStatus="در حال پردازش",
                TotalAmount=request.CartItems.Sum(item => item.Price* item.Quantity),
                OrderItems=request.CartItems.Select(item => new OrderItem
                {
                    ProductId=item.ProductId,
                    Quantity=item.Quantity,
                    Price=item.Price
                }).ToList()

            };
            var newOrder = await _unitOfWork.OrderRepository.AddAsync(order);
            await _unitOfWork.CompleteAsync();
            return newOrder.Id;
        }
    }
}
