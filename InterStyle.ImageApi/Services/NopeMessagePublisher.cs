namespace InterStyle.ImageApi.Services;

public sealed class NopeMessagePublisher : IMessagePublisher
{
    public Task PublishAsync(object message, CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}
