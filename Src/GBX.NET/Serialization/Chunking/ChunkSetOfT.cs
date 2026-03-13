using GBX.NET.Managers;
using System.Collections;

namespace GBX.NET.Serialization.Chunking;

/// <summary>
/// A set of chunks.
/// </summary>
public interface IChunkSet<TKind> : ICollection<TKind>, IEnumerable<TKind>, IEnumerable where TKind : IChunk
{
    IComparer<TKind> Comparer { get; }

    /// <summary>
    /// Creates a new chunk using the ID.
    /// </summary>
    /// <param name="chunkId">ID of the chunk.</param>
    /// <param name="preferHeaderChunks">Whether to prefer creating header chunks if available.</param>
    /// <returns>A new chunk instance.</returns>
    TKind Create(uint chunkId, bool preferHeaderChunks = false);

    /// <summary>
    /// Creates a new chunk using the chunk type.
    /// </summary>
    /// <typeparam name="T">Type of the chunk.</typeparam>
    /// <returns>A new chunk instance.</returns>
    T Create<T>() where T : TKind, new();

    /// <summary>
    /// Removes a chunk using the ID.
    /// </summary>
    /// <param name="chunkId">ID of the chunk.</param>
    /// <returns>True if the chunk was successfully removed, otherwise false.</returns>
    bool Remove(uint chunkId);

    /// <summary>
    /// Removes a chunk using the chunk type.
    /// </summary>
    /// <typeparam name="T">Type of the chunk.</typeparam>
    /// <returns>True if the chunk was successfully removed, otherwise false.</returns>
    bool Remove<T>() where T : TKind;

    /// <summary>
    /// Gets a chunk using the ID.
    /// </summary>
    /// <param name="chunkId">ID of the chunk.</param>
    /// <returns>A new chunk instance if available, otherwise null.</returns>
    TKind? Get(uint chunkId);

    /// <summary>
    /// Gets a chunk using the chunk type.
    /// </summary>
    /// <typeparam name="T">Type of the chunk.</typeparam>
    /// <returns>A new chunk instance if available, otherwise null.</returns>
    T? Get<T>() where T : TKind;
}

internal class ChunkSet<TKind> : IChunkSet<TKind> where TKind : IChunk
{
    private readonly List<TKind> chunks = [];
    private readonly Dictionary<uint, TKind> chunksById = [];
    private readonly Dictionary<Type, TKind> chunksByType = [];

    public virtual IComparer<TKind> Comparer => ChunkComparer<TKind>.Default;

#if NET9_0_OR_GREATER
    private readonly Lock _lock = new();
#else
    private readonly object _lock = new();
#endif

    public int Count => chunks.Count;
    public bool IsReadOnly => false;

    protected virtual TKind New(uint chunkId, bool preferHeaderChunks)
    {
        if (preferHeaderChunks)
        {
            return (TKind)(ClassManager.NewHeaderChunk(chunkId) ?? ClassManager.NewChunk(chunkId) ?? throw new Exception($"Chunk 0x{chunkId:X8} is not supported."));
        }
        else
        {
            return (TKind)(ClassManager.NewChunk(chunkId) ?? ClassManager.NewHeaderChunk(chunkId) ?? throw new Exception($"Chunk 0x{chunkId:X8} is not supported."));
        }
    }

    public bool Add(TKind chunk)
    {
        if (chunk is null) throw new ArgumentNullException(nameof(chunk));

        var chunkType = chunk.GetType();

        if (chunksByType.ContainsKey(chunkType))
        {
            return false;
        }

        chunksById[chunk.Id] = chunk; // overwriting IDs is intentional as some chunks share the same ID with header chunks
        chunksByType[chunkType] = chunk;

        var index = chunks.BinarySearch(chunk, Comparer);
        if (index < 0) index = ~index; // Bitwise complement gets the exact insertion point

        chunks.Insert(index, chunk);
        return true;
    }

    /// <summary>
    /// Add the chunk without checking for duplicates or maintaining order. This is used internally when creating chunks to avoid unnecessary overhead.
    /// </summary>
    /// <param name="chunk"></param>
    /// <exception cref="ArgumentNullException"></exception>
    internal void AddInternal(TKind? chunk)
    {
        if (chunk is null) return;

        chunksById[chunk.Id] = chunk;
        chunksByType[chunk.GetType()] = chunk;

        chunks.Add(chunk);
    }

    public TKind Create(uint chunkId, bool preferHeaderChunks = false)
    {
        if (chunksById.TryGetValue(chunkId, out var chunk))
        {
            return chunk;
        }

        chunk = New(chunkId, preferHeaderChunks);

        Add(chunk);

        return chunk;
    }

    public T Create<T>() where T : TKind, new()
    {
        if (chunksByType.TryGetValue(typeof(T), out var chunk))
        {
            return (T)chunk;
        }

        var newChunk = new T();
        Add(newChunk);

        return newChunk;
    }

    public TKind? Get(uint chunkId)
    {
        return chunksById.TryGetValue(chunkId, out var chunk) ? chunk : default;
    }

    public IChunk? Get(Type chunkType)
    {
        return chunksByType.TryGetValue(chunkType, out var chunk) ? chunk : null;
    }

    public T? Get<T>() where T : TKind
    {
        return (T?)Get(typeof(T));
    }

    public bool Remove(uint chunkId)
    {
        if (!chunksById.TryGetValue(chunkId, out TKind? chunk))
        {
            return false;
        }

        chunksById.Remove(chunkId);
        chunksByType.Remove(chunk.GetType());

        chunks.Remove(chunk);
        return true;
    }

    public bool Remove(Type chunkType)
    {
        if (!chunksByType.TryGetValue(chunkType, out var chunk))
        {
            return false;
        }

        chunksById.Remove(chunk.Id);
        chunksByType.Remove(chunkType);

        chunks.Remove(chunk);
        return true;
    }

    public bool Remove<T>() where T : TKind
    {
        return Remove(typeof(T));
    }

    public bool Remove(TKind chunk)
    {
        if (chunk is null || !chunksById.ContainsKey(chunk.Id))
        {
            return false;
        }

        chunksById.Remove(chunk.Id);
        chunksByType.Remove(chunk.GetType());

        chunks.Remove(chunk);
        return true;
    }

    public void Clear()
    {
        chunks.Clear();
        chunksById.Clear();
        chunksByType.Clear();
    }

    public bool Contains(TKind item)
    {
        return chunks.Contains(item);
    }

    public bool Contains(uint chunkId)
    {
        return chunksById.ContainsKey(chunkId);
    }

    public bool Contains(Type chunkType)
    {
        return chunksByType.ContainsKey(chunkType);
    }

    public bool Contains<T>() where T : TKind
    {
        return Contains(typeof(T));
    }

    public void CopyTo(TKind[] array, int arrayIndex)
    {
        chunks.CopyTo(array, arrayIndex);
    }

    public void ExceptWith(IEnumerable<TKind> other)
    {
        foreach (var chunk in other)
        {
            Remove(chunk.Id);
        }
    }

    public IEnumerator<TKind> GetEnumerator()
    {
        return chunks.GetEnumerator();
    }

    public bool Overlaps(IEnumerable<TKind> other)
    {
        foreach (var chunk in other)
        {
            if (Contains(chunk))
            {
                return true;
            }
        }

        return false;
    }

    void ICollection<TKind>.Add(TKind item)
    {
        Add(item);
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}