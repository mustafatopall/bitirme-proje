namespace Efatura.Shared.Contracts;

public delegate Task<bool> MesajHandler(EfaturaMesaj mesaj, CancellationToken cancellationToken);

public interface IMessagePublisher
{
    Task<bool> PublishAsync(string topic, EfaturaMesaj mesaj, CancellationToken cancellationToken = default);
}

public interface IMessageConsumer : IDisposable
{
    Task SubscribeAsync(string topic, MesajHandler handler, CancellationToken cancellationToken = default);
}
