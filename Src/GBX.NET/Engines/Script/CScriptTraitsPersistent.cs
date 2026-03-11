namespace GBX.NET.Engines.Script;

public partial class CScriptTraitsPersistent
{
    public Dictionary<string, Dictionary<string, CScriptTraitsMetadata.ScriptTrait>> PersistentTraits { get; set; } = [];

    public partial class Chunk11001000 : IVersionable
    {
        public int Version { get; set; }

        public override void Read(CScriptTraitsPersistent n, GbxReader r)
        {
            Version = r.ReadInt32();

            var count = r.ReadInt32();
            n.PersistentTraits = new(count);

            for (int i = 0; i < count; i++)
            {
                var name = r.ReadString();
                n.PersistentTraits.Add(name, CScriptTraitsMetadata.ReadTraits(r, Version - 1));
            }
        }

        public override void Write(CScriptTraitsPersistent n, GbxWriter w)
        {
            w.Write(Version);

            w.Write(n.PersistentTraits.Count);
            foreach (var kvp in n.PersistentTraits)
            {
                w.Write(kvp.Key);
                CScriptTraitsMetadata.WriteTraits(w, Version - 1, kvp.Value);
            }
        }
    }
}
