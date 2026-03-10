namespace GBX.NET;

public sealed class EncapsulatedData(byte[] data, Exception? exception)
{
    public byte[] Data { get; } = data;
    public Exception? Exception { get; set; } = exception;

    public bool Parsed { get; set; }
}
