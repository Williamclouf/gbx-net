using GBX.NET.Managers;

namespace GBX.NET.Serialization.Chunking;

internal class ChunkComparer<TChunk> : IComparer<TChunk> where TChunk : IChunk
{
    public static readonly ChunkComparer<TChunk> Default = new();

    public virtual int Compare(TChunk? x, TChunk? y)
    {
        if (ReferenceEquals(x, y)) return 0;
        if (x is null) return -1;
        if (y is null) return 1;

        // Header chunks at the top
        var xIsHeader = x is IHeaderChunk;
        var yIsHeader = y is IHeaderChunk;
        if (xIsHeader && !yIsHeader) return -1;
        if (!xIsHeader && yIsHeader) return 1;

        // Base class chunks at the top
        var xIsBase = IsBaseClassId(x.Id);
        var yIsBase = IsBaseClassId(y.Id);
        if (xIsBase && !yIsBase) return -1;
        if (!xIsBase && yIsBase) return 1;

        // Typical ordering by ID
        return x.Id.CompareTo(y.Id);
    }

    protected bool IsBaseClassId(uint chunkId)
    {
        var classId = chunkId & 0xFFFFF000;
        return false;
    }
}