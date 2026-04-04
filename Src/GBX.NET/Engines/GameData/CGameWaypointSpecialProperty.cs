namespace GBX.NET.Engines.GameData;

public partial class CGameWaypointSpecialProperty
{
    private CScriptTraitsMetadata? scriptMetadata;

    [AppliedWithChunk<Chunk2E009001>]
    public CScriptTraitsMetadata? ScriptMetadata { get => scriptMetadata; set => scriptMetadata = value; }

    public partial class Chunk2E009001 : IVersionable
    {
        public int Version { get; set; }

        public override void ReadWrite(CGameWaypointSpecialProperty n, GbxReaderWriter rw)
        {
            rw.VersionInt32(this);

            if (rw.Boolean(n.scriptMetadata is not null))
            {
                rw.Encapsulated(rw =>
                {
                    rw.Node<CScriptTraitsMetadata>(ref n.scriptMetadata);
                });
            }
        }
    }
}
