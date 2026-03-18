namespace InterStyle.ImageApi.Services;

public interface IMessagePublisher
{
    public Task PublishAsync(object message, CancellationToken cancellationToken);
}
