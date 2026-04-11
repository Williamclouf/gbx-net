namespace GBX.NET.Engines.Game;

public partial class CGameBuddy
{
    private string? nickName;
    [SupportsFormatting]
    public string? NickName { get => nickName; set => nickName = value; }
}
