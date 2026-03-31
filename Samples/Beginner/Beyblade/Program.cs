using GBX.NET;
using GBX.NET.Engines.Game;
using GBX.NET.Engines.Scene;
using GBX.NET.LZO;
using GBX.NET.ZLib;
using System.Numerics;

Gbx.LZO = new Lzo();
Gbx.ZLib = new ZLib();

var gbx = Gbx.Parse(args[0]);

var ghosts = new List<CGameCtnGhost>();

switch (gbx.Node)
{
    case CGameCtnReplayRecord replay:
        ghosts.AddRange(replay.GetGhosts());
        break;
    case CGameCtnGhost ghost:
        ghosts.Add(ghost);
        break;
    case CGameCtnMediaClip clip:
        ghosts.AddRange(clip.GetGhosts());
        break;
    default:
        Console.WriteLine("Unsupported file type.");
        break;
}

foreach (var (i, ghost) in ghosts.Index())
{
    Beyblade(ghost);

    ghost.Save($"{gbx.GetFileNameWithoutExtension()}-beyblade-{i}.Ghost.Gbx", new()
    {
        ClassIdRemapMode = gbx.ClassIdRemapMode,
        PackDescVersion = gbx.PackDescVersion,
    });
}

static void Beyblade(CGameCtnGhost ghost)
{
    var spin = Quat.Identity;

    float angularSpeed = 15f;

    // TM1-2
    foreach (var sample in ghost.SampleData.Samples.Skip(1))
    {
        var delta = (Quat)Quaternion.CreateFromYawPitchRoll(angularSpeed, 0, 0);

        spin *= delta;

        sample.Rotation *= spin;
    }

    // TM2020
    var samples = ghost.RecordData?
        .EntList.SelectMany(x => x.Samples)
        .OfType<CSceneVehicleVis.EntRecordDelta>();

    foreach (var sample in samples?.Skip(1) ?? [])
    {
        var delta = (Quat)Quaternion.CreateFromYawPitchRoll(angularSpeed, 0, 0);

        spin *= delta;

        sample.Rotation *= spin;
    }
}