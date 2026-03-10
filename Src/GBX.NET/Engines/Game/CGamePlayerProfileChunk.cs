namespace GBX.NET.Engines.Game;

public partial class CGamePlayerProfileChunk
{
    public string ChunkName { get; set; } = string.Empty;
    public string ChunkGroup { get; set; } = string.Empty;
    public string Checksum { get; set; } = string.Empty;
    public DateTimeOffset LastUpdated { get; set; }
    public int ArchiveVersion { get; set; }
    public int? SkipArchiveVersion { get; set; }
    public DateTimeOffset? UnknownTimestamp { get; set; }
}
