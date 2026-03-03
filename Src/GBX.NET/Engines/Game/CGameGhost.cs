namespace GBX.NET.Engines.Game;

public partial class CGameGhost
{
    private Data? sampleData;

    public byte[]? RawData { get; set; }

    private ZlibData? compressedData;
    public ZlibData? CompressedData { get => compressedData; set => compressedData = value; }

    private readonly object parseLock = new();

    /// <exception cref="ZLibNotDefinedException">Zlib is not defined.</exception>
    [AppliedWithChunk<Chunk0303F003>]
    [AppliedWithChunk<Chunk0303F005>]
    [AppliedWithChunk<Chunk0303F006>]
    public Data SampleData
    {
        get
        {
            if (sampleData is not null)
            {
                return sampleData;
            }

            if (CompressedData is null)
            {
                throw new InvalidOperationException("No compressed data available to parse.");
            }

            lock (parseLock)
            {
                if (sampleData is not null)
                {
                    return sampleData;
                }

                using var reader = CompressedData.OpenDecompressedReader();
                sampleData = new Data();
                sampleData.Read(reader);
                return sampleData;
            }
        }
    }

    public partial class Chunk0303F003
    {
        public int[]? Times;

        public override void Read(CGameGhost n, GbxReader r)
        {
            n.RawData = r.ReadData();
            n.sampleData = new Data()
            {
                Offsets = r.ReadArray<int>()
            };
            Times = r.ReadArray<int>();
            n.sampleData.IsFixedTimeStep = r.ReadBoolean();
            n.sampleData.SamplePeriod = r.ReadTimeInt32();
            n.sampleData.Version = r.ReadInt32();
        }

        public override void Write(CGameGhost n, GbxWriter w)
        {
            w.Write(n.RawData);
            w.WriteArray(n.sampleData?.Offsets);
            w.WriteArray(Times);
            w.Write(n.sampleData?.IsFixedTimeStep ?? false);
            w.Write(n.sampleData?.SamplePeriod ?? TimeInt32.FromMilliseconds(100));
            w.Write(n.sampleData?.Version ?? 0);
        }
    }

    public partial class Chunk0303F005
    {
        public override void Read(CGameGhost n, GbxReader r)
        {
            n.compressedData = r.ReadZlibData();
        }

        public override void Write(CGameGhost n, GbxWriter w)
        {
            w.WriteZlibData(n.compressedData, n.SampleData);
        }
    }
}
