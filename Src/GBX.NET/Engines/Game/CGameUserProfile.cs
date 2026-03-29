namespace GBX.NET.Engines.Game;

public partial class CGameUserProfile
{
    public partial class Chunk031CC001 : IVersionable
    {
        public int Version { get; set; }

        public RawData U01 = new([], null);

        public override void Read(CGameUserProfile n, GbxReader r)
        {
            Version = r.ReadInt32();
            U01 = r.ReadEncapsulated();
        }

        public override void Write(CGameUserProfile n, GbxWriter w)
        {
            w.Write(Version);
            w.WriteEncapsulated(U01);
        }
    }
}
