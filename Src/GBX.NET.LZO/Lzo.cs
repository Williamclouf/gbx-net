using GBX.NET.Extensions;

namespace GBX.NET.LZO;

public sealed class Lzo : ILzo
{
    public byte[] Compress(byte[] data)
    {
        return SharpLzo.Lzo.Compress(SharpLzo.CompressionMode.Lzo1x_999, data);
    }

    public void Decompress(in Span<byte> input, byte[] output)
    {
        var result = SharpLzo.Lzo.TryDecompress(input, input.Length, output, out var dstLength);
        
        if (result != SharpLzo.LzoResult.OK)
        {
            throw new SharpLzo.LzoException(result);
        }

        if (dstLength == 0)
        {
            throw new InvalidOperationException("Decompression resulted in zero-length output.");
        }

        if (dstLength != output.Length)
        {
            throw new InvalidOperationException($"Decompression resulted in unexpected output length. Expected: {output.Length}, Actual: {dstLength}");
        }
    }
}
