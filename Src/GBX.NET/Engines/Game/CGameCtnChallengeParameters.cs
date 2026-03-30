namespace GBX.NET.Engines.Game;

public partial class CGameCtnChallengeParameters
{
    private CGameCtnGhost? raceValidateGhost;
    [AppliedWithChunk<Chunk0305B00D>]
    [AppliedWithChunk<Chunk0305B00F>]
    public CGameCtnGhost? RaceValidateGhost { get => raceValidateGhost; set => raceValidateGhost = value; }

    public partial class Chunk0305B00F
    {
        public override void ReadWrite(CGameCtnChallengeParameters n, GbxReaderWriter rw)
        {
            rw.Encapsulated(rw =>
            {
                rw.NodeRef<CGameCtnGhost>(ref n.raceValidateGhost);
            });
        }
    }
}
