namespace GBX.NET.Engines.Game;

public partial class CGamePlayerProfileChunk_InputBindingsConfig
{
    private CInputBindingsConfig? config;
    [AppliedWithChunk<Chunk0312F000>]
    public CInputBindingsConfig? Config { get => config; set => config = value; }

    public partial class Chunk0312F000 : IVersionable
    {
        public int Version { get; set; }

        public override void ReadWrite(CGamePlayerProfileChunk_InputBindingsConfig n, GbxReaderWriter rw)
        {
            rw.VersionInt32(this);
            rw.Encapsulated(rw =>
            {
                rw.NodeRef<CInputBindingsConfig>(ref n.config);
            });
        }
    }
}
