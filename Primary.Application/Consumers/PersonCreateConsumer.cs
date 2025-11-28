using MassTransit;
using Primary.Application.Contracts;

namespace Primary.Application.Consumers
{
    public class PersonCreateConsumer(ILogger<PersonCreateConsumer> logger) : IConsumer<PersonCreate>
    {
        public Task Consume(ConsumeContext<PersonCreate> context)
        {
            logger.LogInformation("Person created: @{Message}", context.Message);

            return Task.CompletedTask;
        }
    }
}
