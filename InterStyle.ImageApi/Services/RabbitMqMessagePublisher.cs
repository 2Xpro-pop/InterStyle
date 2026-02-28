using RabbitMQ.Client;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using System.Text.Json;

namespace InterStyle.ImageApi.Services;

public sealed class RabbitMqMessagePublisher : IMessagePublisher, IAsyncDisposable
{
    private readonly ConnectionFactory factory;
    private IConnection? connection;
    private readonly string exchangeName = "interstyle.events";

    public RabbitMqMessagePublisher(string connectionString)
    {
        factory = new ConnectionFactory
        {
            Uri = new Uri(connectionString)
        };
    }

    private async ValueTask<IConnection> GetConnection()
    {
        if (connection is { IsOpen: true })
            return connection;

        connection = await factory.CreateConnectionAsync();
        return connection;
    }

    [UnconditionalSuppressMessage(
        "Trimming",
        "IL2026",
        Justification = "Serialization is safe because all message types are registered in AppJsonSerializerContext")]
    public async Task PublishAsync(object message, CancellationToken cancellationToken)
    {
        var conn = await GetConnection();
        await using var channel = await conn.CreateChannelAsync(cancellationToken: cancellationToken);

        await channel.ExchangeDeclareAsync(
            exchangeName,
            ExchangeType.Topic,
            durable: true,
            cancellationToken: cancellationToken);

        var json = JsonSerializer.Serialize(
            message,
            message.GetType(),
            AppJsonSerializerContext.Default);

        var body = Encoding.UTF8.GetBytes(json);

        var props = new BasicProperties
        {
            Persistent = true,
            ContentType = "application/json"
        };

        var routingKey = message.GetType().Name;

        await channel.BasicPublishAsync(
            exchange: exchangeName,
            routingKey: routingKey,
            mandatory: false,
            basicProperties: props,
            body: body,
            cancellationToken: cancellationToken);
    }

    public async ValueTask DisposeAsync()
    {
        if (connection != null)
            await connection.DisposeAsync();
    }
}