using MediatR;
using OrderManagement.Domain.Enums;

namespace OrderManagement.Application.Orders.Queries.GetOrder;

public sealed record GetOrderQuery(Guid OrderId) : IRequest<OrderDetailsDto?>;

public sealed record OrderDetailsDto(
    Guid Id,
    Guid CustomerId,
    OrderStatus Status,
    DateTime CreatedAt,
    decimal TotalAmount,
    IReadOnlyList<OrderItemDto> Items);

public sealed record OrderItemDto(
    Guid Id,
    string ProductName,
    int Quantity,
    decimal UnitPrice,
    decimal Total);
