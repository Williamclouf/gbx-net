namespace GBX.NET.Engines.Game;

public partial class CGameCtnBlock
{
    public class TMUnlimiter
    {
        public Byte3 OverOverSizeChunk { get; set; }
        public bool IsInverted { get; set; }
        public Int3 Offset { get; set; }
        public Int3 Rotation { get; set; }
    }
}
