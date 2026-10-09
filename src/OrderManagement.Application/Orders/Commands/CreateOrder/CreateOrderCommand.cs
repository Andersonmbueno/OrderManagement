using MediatR;

namespace OrderManagement.Application.Orders.Commands.CreateOrder;

public sealed record CreateOrderCommand(
    Guid CustomerId,
    IReadOnlyList<CreateOrderItem> Items) : IRequest<Guid>;

public sealed record CreateOrderItem(
    string ProductName,
    int Quantity,
    decimal UnitPrice);
