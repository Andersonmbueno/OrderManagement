using MediatR;
using OrderManagement.Application.Common;

namespace OrderManagement.Application.Orders.Queries.GetOrder;

public sealed class GetOrderHandler : IRequestHandler<GetOrderQuery, OrderDetailsDto?>
{
    private readonly IOrderRepository _repository;

    public GetOrderHandler(IOrderRepository repository)
    {
        _repository = repository;
    }

    public async Task<OrderDetailsDto?> Handle(
        GetOrderQuery request,
        CancellationToken cancellationToken)
    {
        var order = await _repository.GetByIdAsync(request.OrderId, cancellationToken);

        if (order is null)
            return null;

        return new OrderDetailsDto(
            order.Id,
            order.CustomerId,
            order.Status,
            order.CreatedAt,
            order.TotalAmount,
            order.Items.Select(item => new OrderItemDto(
                item.Id,
                item.ProductName,
                item.Quantity,
                item.UnitPrice,
                item.Total)).ToList());
    }
}
