using FluentAssertions;
using FluentValidation;
using OrderManagement.Application.Common;
using OrderManagement.Application.Orders.Commands.CreateOrder;
using MediatR;
using Xunit;

namespace OrderManagement.UnitTests;

public sealed class ValidationBehaviorTests
{
    [Fact]
    public async Task Should_Stop_Request_When_Validation_Fails()
    {
        var validators = new IValidator<CreateOrderCommand>[]
        {
            new CreateOrderValidator()
        };

        var behavior = new ValidationBehavior<CreateOrderCommand, Guid>(validators);
        var command = new CreateOrderCommand(Guid.Empty, Array.Empty<CreateOrderItem>());
        var nextCalled = false;

        var act = () => behavior.Handle(
            command,
            _ =>
            {
                nextCalled = true;
                return Task.FromResult(Guid.NewGuid());
            },
            CancellationToken.None);

        await act.Should().ThrowAsync<ValidationException>();
        nextCalled.Should().BeFalse();
    }
}
