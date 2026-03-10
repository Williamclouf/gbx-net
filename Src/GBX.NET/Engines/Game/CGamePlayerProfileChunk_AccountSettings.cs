namespace GBX.NET.Engines.Game;

public partial class CGamePlayerProfileChunk_AccountSettings
{
    public bool LoginValidated
    {
        get => BitHelper.GetBit(flags, 0);
        set => flags = BitHelper.SetBit(flags, 0, value);
    }

    public bool RememberOnlinePassword
    {
        get => BitHelper.GetBit(flags, 1);
        set => flags = BitHelper.SetBit(flags, 1, value);
    }

    public bool AutoConnect
    {
        get => BitHelper.GetBit(flags, 2);
        set => flags = BitHelper.SetBit(flags, 2, value);
    }

    public bool AskForAccountConversion
    {
        get => BitHelper.GetBit(flags, 3);
        set => flags = BitHelper.SetBit(flags, 3, value);
    }

    public bool UnlockAllCheats
    {
        get => BitHelper.GetBit(flags2, 0);
        set => flags2 = BitHelper.SetBit(flags2, 0, value);
    }

    public bool FriendsCheat
    {
        get => BitHelper.GetBit(flags2, 1);
        set => flags2 = BitHelper.SetBit(flags2, 1, value);
    }

    public partial class Chunk0312C005 : IVersionable
    {
        public int Version { get; set; }

        public ulong U02;

        public override void ReadWrite(CGamePlayerProfileChunk_AccountSettings n, GbxReaderWriter rw)
        {
            rw.VersionInt32(this);

            if (Version < 2)
            {
                rw.UInt64(ref U02);
                rw.ArrayNodeRef_deprec<CGameNetOnlineMessage>(ref n.inboxMessages!);
                rw.ArrayNodeRef_deprec<CGameNetOnlineMessage>(ref n.readMessages!);
                rw.ArrayNodeRef_deprec<CGameNetOnlineMessage>(ref n.outboxMessages!);
                return;
            }

            rw.Encapsulated(rw =>
            {
                rw.UInt64(ref U02);
                rw.ArrayNodeRef_deprec<CGameNetOnlineMessage>(ref n.inboxMessages!);
                rw.ArrayNodeRef_deprec<CGameNetOnlineMessage>(ref n.readMessages!);
                rw.ArrayNodeRef_deprec<CGameNetOnlineMessage>(ref n.outboxMessages!);
            });
        }
    }
}
