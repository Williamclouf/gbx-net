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
        var classX = x.Id & 0xFFFFF000;
        var classY = y.Id & 0xFFFFF000;

        if (classX != classY)
        {
            if (IsSubclassOf(classY, classX)) return -1;
            if (IsSubclassOf(classX, classY)) return 1;
        }

        // Typical ordering by ID
        return x.Id.CompareTo(y.Id);
    }
}