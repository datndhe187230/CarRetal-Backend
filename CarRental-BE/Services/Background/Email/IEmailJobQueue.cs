using System.Threading.Channels;

namespace CarRental_BE.Services.Background.Email;

public interface IEmailJobQueue
{
    ValueTask EnqueueAsync(StatusChangeEmailJob job, CancellationToken ct = default);
    ValueTask<StatusChangeEmailJob> DequeueAsync(CancellationToken ct);
}

internal sealed class EmailJobQueue : IEmailJobQueue
{
    private readonly Channel<StatusChangeEmailJob> _channel;

    public EmailJobQueue()
    {
        _channel = Channel.CreateUnbounded<StatusChangeEmailJob>(new UnboundedChannelOptions
        {
            SingleReader = true,
            SingleWriter = false,
            AllowSynchronousContinuations = true
        });
    }

    public ValueTask EnqueueAsync(StatusChangeEmailJob job, CancellationToken ct = default)
        => _channel.Writer.WriteAsync(job, ct);

    public ValueTask<StatusChangeEmailJob> DequeueAsync(CancellationToken ct)
        => _channel.Reader.ReadAsync(ct);
}