using Xunit;
using FluentAssertions;
using OrderManagement.Domain.Entities;
using OrderManagement.Domain.Enums;

namespace OrderManagement.UnitTests;

public sealed class OrderDomainTests
{
    [Fact]
    public void TotalAmount_Should_Be_Calculated_In_Domain()
    {
        var order = Order.Create(
            Guid.NewGuid(),
            new[]
            {
                new OrderItem("A", 2, 10m),
                new OrderItem("B", 3, 5m)
            });

        order.TotalAmount.Should().Be(35m);
    }

    [Fact]
    public void Order_Should_Start_As_Pending()
    {
        var order = Order.Create(
            Guid.NewGuid(),
            new[] { new OrderItem("A", 1, 10m) });

        order.Status.Should().Be(OrderStatus.Pending);
    }
}
