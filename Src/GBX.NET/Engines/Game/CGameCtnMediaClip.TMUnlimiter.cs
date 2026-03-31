namespace GBX.NET.Engines.Game;

public partial class CGameCtnMediaClip
{
    public class TMUnlimiter
    {
        public LegacyResource? Resource { get; set; }

        public abstract record LegacyResource;

        public sealed record LegacyParameterSet : LegacyResource
        {
            public Parameter[] Parameters { get; set; } = [];
        }

        public sealed record Parameter
        {
            public byte CatalogIndex { get; set; }
            public byte FunctionIndex { get; set; }
            public float Value { get; set; }
        }

        public sealed record LegacyScript : LegacyResource
        {
            public byte[] ByteCode { get; set; } = [];
        }
    }
}
