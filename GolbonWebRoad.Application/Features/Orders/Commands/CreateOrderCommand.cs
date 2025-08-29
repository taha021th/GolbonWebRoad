using GolbonWebRoad.Application.Dtos.CartItems;
using GolbonWebRoad.Domain.Entities;
using GolbonWebRoad.Domain.Interfaces;
using MediatR;

namespace GolbonWebRoad.Application.Features.Orders.Commands
{
    public class CreateOrderCommand : IRequest<int>
    {
        public string UserId { get; set; }
        public List<CartItemSummaryDto> CartItems { get; set; }
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
            var orderItems = new List<OrderItem>();
            decimal totalAmount = 0;
            foreach (var item in request.CartItems)
            {
                var product = await _unitOfWork.ProductRepository.GetByIdAsync(item.ProductId);
                if (product==null)
                {
                    continue;
                }
                var price = product.Price;
                totalAmount+=price* item.Quantity;
                orderItems.Add(new OrderItem
                {
                    ProductId=item.ProductId,
                    Quantity=item.Quantity
                });
            }
            var order = new Order
            {
                UserId=request.UserId,
                OrderDate=DateTime.UtcNow,
                OrderStatus="در حال پردازش",
                OrderItems=orderItems,
                TotalAmount=totalAmount
            };
            var newOrder = await _unitOfWork.OrderRepository.AddAsync(order);
            await _unitOfWork.CompleteAsync();
            return newOrder.Id;
        }
    }
}
