using MassTransit;
using Microsoft.Extensions.Logging;
using Secondary.Application.Contracts;

namespace Secondary.Application.Consumers
{
    public class OrderCreateConsumer(ILogger<OrderCreateConsumer> logger) : IConsumer<OrderCreate>
    {
        public Task Consume(ConsumeContext<OrderCreate> context)
        {
            logger.LogInformation("Person created: @{Message}", context.Message);

            return Task.CompletedTask;
        }
    }
}
