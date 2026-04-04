namespace GBX.NET.Engines.Game;

public partial class CGameCtnBlock
{
    public class TMUnlimiter
    {
        public Byte3 OverOverSizeChunk { get; set; }
        public Vec3 Offset { get; set; }
        public Vec3 Rotation { get; set; }
        public Vec3 Scale { get; set; } = Vec3.One;
        public string? Group { get; set; }

        public bool IsInverted { get; set; }
        public bool IsVanillaTerrain { get; set; }
        public bool IsSpawnPointFixEnabled { get; set; }
        public bool IsDynamic { get; set; }
        public bool IsInvisible { get; set; }
        public bool IsCollisionDisabled { get; set; }
        public bool IsClassicMode { get; set; }
        public bool IsClassicTerrain { get; set; }

        public bool IsOutsideBoundaries => OverOverSizeChunk != Byte3.Zero;
        public bool IsMoved => Offset != Vec3.Zero;
        public bool IsRotated => Rotation != Vec3.Zero;
        public bool IsScaled => Scale != Vec3.One;
        public bool HasIdentifier => Group is not null;

        public CGameCtnChallenge.TMUnlimiter.Motion? Motion { get; set; }
        public Vec3? OriginOffset { get; set; }
        public List<CGameCtnChallenge.TMUnlimiter.BlockGroup> BlockGroups { get; set; } = [];
        public CGameCtnChallenge.TMUnlimiter.ESpawnPointAlterMethod SpawnPointAlterMethod { get; set; }
        public Vec3? SpawnOffset { get; set; }
        public Vec3? SpawnRotation { get; set; }
        public bool IsNonCollidable { get; set; }
        public CGameCtnChallenge.TMUnlimiter.ERespawnCapability RespawnCapability { get; set; }
        public CGameCtnChallenge.TMUnlimiter.EBlockTriggerActivationMethod BlockTriggerActivationMethod { get; set; }
    }
}
