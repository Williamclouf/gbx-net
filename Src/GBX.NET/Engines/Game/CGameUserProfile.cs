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

    public partial class Chunk031CC009 : IVersionable
    {
        public int Version { get; set; }

        public Id U01;

        public override void ReadWrite(CGameUserProfile n, GbxReaderWriter rw)
        {
            rw.VersionInt32(this);
            rw.Id(ref U01);
        }
    }

    public partial class Chunk031CC01B
    {
        public (int, int, int, int, int, int, int, int)[]? U01;

        public override void ReadWrite(CGameUserProfile n, GbxReaderWriter rw)
        {
            rw.Array<(int, int, int, int, int, int, int, int)>(ref U01, length: n.unknowns5?.Length ?? 0);
        }
    }

    public partial class Chunk031CC021
    {
        public int[]? U01;

        public override void ReadWrite(CGameUserProfile n, GbxReaderWriter rw)
        {
            rw.Array<int>(ref U01, length: n.unknowns5?.Length ?? 0);
        }
    }
}
