namespace GBX.NET.Serialization.Chunking;

internal readonly record struct HeaderChunkInfo([property: Hexadecimal] uint Id, int Size, bool IsHeavy);
