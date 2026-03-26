#if NETSTANDARD2_0
namespace GBX.NET.Serialization;

internal static partial class StreamExtensions
{
    [Zomp.SyncMethodGenerator.CreateSyncVersion]
    public static async Task<int> ReadAtLeastAsync(this Stream stream, byte[] buffer, int minimumBytes, bool throwOnEndOfStream = true, CancellationToken cancellationToken = default)
    {
        var totalRead = 0;

        while (totalRead < minimumBytes)
        {
            var read = await stream.ReadAsync(buffer, totalRead, buffer.Length - totalRead, cancellationToken);

            if (read == 0)
            {
                if (throwOnEndOfStream)
                {
                    throw new EndOfStreamException();
                }

                return totalRead;
            }

            totalRead += read;
        }

        return totalRead;
    }

    [Zomp.SyncMethodGenerator.CreateSyncVersion]
    public static async Task ReadExactlyAsync(this Stream stream, byte[] buffer, CancellationToken cancellationToken = default)
    {
        _ = await ReadAtLeastAsync(stream, buffer, buffer.Length, cancellationToken: cancellationToken);
    }
}
#endif