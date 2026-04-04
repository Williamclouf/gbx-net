namespace GBX.NET.Engines.Hms;

public partial class CHmsItem
{
    protected override bool OnRawChunkIdRead(uint rawChunkId)
    {
        return rawChunkId != 0;
    }
}
