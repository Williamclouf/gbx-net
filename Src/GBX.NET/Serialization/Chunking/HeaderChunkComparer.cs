namespace GBX.NET.Serialization.Chunking;

internal sealed class HeaderChunkComparer : ChunkComparer<IHeaderChunk>
{
    public static new readonly HeaderChunkComparer Default = new();

    public override int Compare(IHeaderChunk? x, IHeaderChunk? y)
    {
        if (ReferenceEquals(x, y)) return 0;
        if (x is null) return -1;
        if (y is null) return 1;

        // Base class chunks at the top
        var xIsBase = IsBaseClassId(x.Id);
        var yIsBase = IsBaseClassId(y.Id);
        if (xIsBase && !yIsBase) return -1;
        if (!xIsBase && yIsBase) return 1;

        // Typical ordering by ID
        return x.Id.CompareTo(y.Id);
    }
}