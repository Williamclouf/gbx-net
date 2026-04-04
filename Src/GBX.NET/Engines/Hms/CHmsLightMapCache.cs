namespace GBX.NET.Engines.Hms;

public partial class CHmsLightMapCache
{
    public Id Collection { get; set; }

    public sealed partial class Frame
    {
        [WebpData] // NOT always but mostly better to have
        public byte[]? Data { get; set; }

        [WebpData] // NOT always but mostly better to have
        public byte[]? Data2 { get; set; }

        [WebpData] // NOT always but mostly better to have
        public byte[]? Data3 { get; set; }
    }
}
