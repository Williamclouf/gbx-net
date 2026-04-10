namespace GBX.NET.Engines.Game;

public partial class CGameNetOnlineMessage
{
    private string? message;
    [AppliedWithChunk<Chunk03028000>]
    [SupportsFormatting]
    public string? Message { get => message; set => message = value; }
}
