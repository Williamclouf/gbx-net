namespace GBX.NET.Engines.Game;

public partial class CGamePlayerProfileChunk_ScriptPersistentTraits
{
    private CScriptTraitsPersistent? scriptPersistentTraits;
    public CScriptTraitsPersistent? ScriptPersistentTraits { get => scriptPersistentTraits; set => scriptPersistentTraits = value; }

    public partial class Chunk03170000 : IVersionable
    {
        public int Version { get; set; }

        public override void ReadWrite(CGamePlayerProfileChunk_ScriptPersistentTraits n, GbxReaderWriter rw)
        {
            rw.VersionInt32(this);
            rw.Encapsulated(rw =>
            {
                rw.Node<CScriptTraitsPersistent>(ref n.scriptPersistentTraits);
            });
        }
    }
}
