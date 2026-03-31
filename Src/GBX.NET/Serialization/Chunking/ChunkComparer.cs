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

    protected static bool IsSubclassOf(uint classId, uint parentClassId)
    {
        var currentClassId = classId;

        while (true)
        {
            var baseClassId = ClassManager.GetBaseClassId(currentClassId);

            // Reached the top of the hierarchy without finding the parent
            if (baseClassId is null)
            {
                return false;
            }

            // Target parent found in the inheritance chain
            if (baseClassId.Value == parentClassId)
            {
                return true;
            }

            // Prevent infinite loops if a root class points to itself
            if (baseClassId.Value == currentClassId)
            {
                return false;
            }

            currentClassId = baseClassId.Value;
        }
    }
}