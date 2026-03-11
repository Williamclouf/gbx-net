namespace GBX.NET.Engines.Game;

public partial class CGamePlayerProfileChunk_GameScores
{
    private CGamePlayerOfficialScores? officialScores1;
    public CGamePlayerOfficialScores? OfficialScores1 { get => officialScores1; set => officialScores1 = value; }

    private CGamePlayerOfficialScores? officialScores2;
    public CGamePlayerOfficialScores? OfficialScores2 { get => officialScores2; set => officialScores2 = value; }

    public partial class Chunk03146001 : IVersionable
    {
        public int Version { get; set; }

        public override void ReadWrite(CGamePlayerProfileChunk_GameScores n, GbxReaderWriter rw)
        {
            rw.VersionInt32(this);

            rw.Encapsulated(rw =>
            {
                rw.NodeRef<CGamePlayerOfficialScores>(ref n.officialScores1);
            });
        }
    }

    public partial class Chunk03146004 : IVersionable
    {
        public int Version { get; set; }

        public override void ReadWrite(CGamePlayerProfileChunk_GameScores n, GbxReaderWriter rw)
        {
            rw.VersionInt32(this);

            rw.Encapsulated(rw =>
            {
                rw.NodeRef<CGamePlayerOfficialScores>(ref n.officialScores2);
            });
        }
    }

    public partial class STrainingMedalsScores
    {
        private string? u01;
        public string? U01 { get => u01; set => u01 = value; }

        private int u02;
        public int U02 { get => u02; set => u02 = value; }

        private CGamePlayerOfficialScores? officialScores;
        public CGamePlayerOfficialScores? OfficialScores { get => officialScores; set => officialScores = value; }

        public void ReadWrite(GbxReaderWriter rw, int v = 0)
        {
            rw.Id(ref u01);
            rw.Int32(ref u02);

            rw.Encapsulated(rw =>
            {
                rw.NodeRef<CGamePlayerOfficialScores>(ref officialScores);
            });
        }
    }
}
