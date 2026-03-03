namespace GBX.NET;

public sealed class ZlibData(int uncompressedSize, byte[] data, Exception? exception)
{
    public int UncompressedSize { get; } = uncompressedSize;
    public byte[] Data { get; } = data;
    public Exception? Exception { get; } = exception;

    public bool Parsed { get; set; }

    internal static GbxReader OpenDecompressedReader(int uncompressedSize, byte[] data, GbxReader? referenceReader = null)
    {
        using var compressedStream = new MemoryStream(data);
        var uncompressedStream = new MemoryStream(uncompressedSize);

        Gbx.ZLib.Decompress(compressedStream, uncompressedStream);

        var rBuffer = new GbxReader(uncompressedStream);

        if (referenceReader is not null)
        {
            rBuffer.LoadFrom(referenceReader);
        }

        uncompressedStream.Position = 0;
        return rBuffer;
    }

    public GbxReader OpenDecompressedReader()
    {
        return OpenDecompressedReader(UncompressedSize, Data);
    }
}
