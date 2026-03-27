namespace GBX.NET.Serialization;

internal sealed partial class BoundedStream : Stream
{
    private readonly Stream baseStream;
    private readonly long length;
    private readonly long startPosition;
    private long remaining;
    private MemoryStream? memoryStream;

    public override bool CanRead => true;
    public override bool CanSeek => true;
    public override bool CanWrite => false;
    public override long Length => length;

    public override long Position
    {
        get => length - remaining;
        set => Seek(value, SeekOrigin.Begin);
    }

    public BoundedStream(Stream baseStream, long length)
    {
        this.baseStream = baseStream ?? throw new ArgumentNullException(nameof(baseStream));
        if (!baseStream.CanRead) throw new ArgumentException("Base stream must be readable.");
        if (length < 0) throw new ArgumentOutOfRangeException(nameof(length));

        this.length = length;
        startPosition = baseStream.CanSeek ? baseStream.Position : 0;
        remaining = length;
    }

#if NET5_0_OR_GREATER
    public override async ValueTask<int> ReadAsync(Memory<byte> buffer, CancellationToken cancellationToken = default)
    {
        if (remaining <= 0) return 0; // End of bounded stream

        int read;

        if (baseStream.CanSeek)
        {
            // Ensure the base stream is at the correct position before reading
            EnsureBaseStreamPosition();

            read = await baseStream.ReadAsync(buffer.Slice(0, (int)Math.Min(buffer.Length, remaining)), cancellationToken);
        }
        else
        {
            if (memoryStream is null)
            {
                var data = new byte[remaining];
                await baseStream.ReadAtLeastAsync(data, data.Length, throwOnEndOfStream: false, cancellationToken);
                memoryStream = new MemoryStream(data);
            }

            read = await memoryStream.ReadAsync(buffer.Slice(0, (int)Math.Min(buffer.Length, remaining)), cancellationToken);
        }

        remaining -= read;
        return read;
    }
#endif

    [Zomp.SyncMethodGenerator.CreateSyncVersion]
    public override async Task<int> ReadAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
    {
        if (remaining <= 0) return 0; // End of bounded stream

        int read;

        if (baseStream.CanSeek)
        {
            // Ensure the base stream is at the correct position before reading
            EnsureBaseStreamPosition();

#if NET5_0_OR_GREATER
            read = await baseStream.ReadAsync(buffer.AsMemory(offset, (int)Math.Min(count, remaining)), cancellationToken);
#else
            read = await baseStream.ReadAsync(buffer, offset, (int)Math.Min(count, remaining), cancellationToken);
#endif
        }
        else
        {
            if (memoryStream is null)
            {
                var data = new byte[remaining];
                await baseStream.ReadAtLeastAsync(data, data.Length, throwOnEndOfStream: false, cancellationToken);
                memoryStream = new MemoryStream(data);
            }

#if NET5_0_OR_GREATER
            read = await memoryStream.ReadAsync(buffer.AsMemory(offset, (int)Math.Min(count, remaining)), cancellationToken);
#else
            read = await memoryStream.ReadAsync(buffer, offset, (int)Math.Min(count, remaining), cancellationToken);
#endif
        }

        remaining -= read;
        return read;
    }

    private void EnsureBaseStreamPosition()
    {
        long expectedPosition = startPosition + (length - remaining);
        if (baseStream.Position != expectedPosition)
        {
            baseStream.Position = expectedPosition;
        }
    }

    public override void Write(byte[] buffer, int offset, int count)
    {
        throw new NotSupportedException();
    }

    public override void Flush()
    {
        throw new NotSupportedException();
    }

    public override long Seek(long offset, SeekOrigin origin)
    {
        var newPosition = origin switch
        {
            SeekOrigin.Begin => offset,
            SeekOrigin.Current => length - remaining + offset,
            SeekOrigin.End => length + offset,
            _ => throw new ArgumentException("Invalid seek origin.", nameof(origin))
        };

        if (newPosition < 0 || newPosition > length)
        {
            throw new IOException("An attempt was made to move the position before the beginning or after the end of the stream.");
        }

        if (baseStream.CanSeek)
        {
            baseStream.Seek(startPosition + newPosition, SeekOrigin.Begin);
        }
        else
        {
            if (memoryStream is null)
            {
                var data = new byte[remaining];
                baseStream.ReadExactly(data);
                memoryStream = new MemoryStream(data);
            }

            memoryStream.Seek(newPosition, SeekOrigin.Begin);
        }

        remaining = length - newPosition;
        return newPosition;
    }

    public override void SetLength(long value)
    {
        throw new NotSupportedException();
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            memoryStream?.Dispose();

            if (remaining > 0)
            {
                throw new InvalidOperationException($"Not all data was read from the bounded stream. {remaining} bytes remaining.");
            }
        }
        base.Dispose(disposing);
    }

#if NET5_0_OR_GREATER
    public override async ValueTask DisposeAsync()
    {
        if (memoryStream is not null)
        {
            await memoryStream.DisposeAsync().ConfigureAwait(false);
        }

        if (remaining > 0)
        {
            throw new InvalidOperationException($"Not all data was read from the bounded stream. {remaining} bytes remaining.");
        }

        await base.DisposeAsync().ConfigureAwait(false);
    }
#endif
}
