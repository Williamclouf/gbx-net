namespace GBX.NET.Engines.Scene;

public partial class CSceneVehicleCar
{
    public interface ISampleRawData
    {
        ushort SpeedForward { get; set; }
        ushort SpeedSideward { get; set; }
        ushort RPM { get; set; }
        ushort FLWheelRotation { get; set; }
        ushort FRWheelRotation { get; set; }
        ushort RRWheelRotation { get; set; }
        ushort RLWheelRotation { get; set; }
        byte Steer { get; set; }
        byte Gas { get; set; }
        byte Brake { get; set; }
        byte U11 { get; set; }
        byte U12 { get; set; }
        byte U13 { get; set; }
        byte U14 { get; set; }
        byte TurboStrength { get; set; }
        byte SteerFront { get; set; }
        byte FLDampenLen { get; set; }
        CPlugSurface.MaterialId FLGroundContactMaterial { get; set; }
        byte FRDampenLen { get; set; }
        CPlugSurface.MaterialId FRGroundContactMaterial { get; set; }
        byte RRDampenLen { get; set; }
        CPlugSurface.MaterialId RRGroundContactMaterial { get; set; }
        byte RLDampenLen { get; set; }
        CPlugSurface.MaterialId RLGroundContactMaterial { get; set; }
        byte U25 { get; set; }
        byte U26 { get; set; }
        byte U27 { get; set; }
        byte? DirtBlend { get; set; }
        byte? U33 { get; set; }
        byte? U34 { get; set; }
        byte? U35 { get; set; }
        (Vec3, Quat, byte)[]? U35_1 { get; set; }
        byte? U36 { get; set; }
        byte? U37 { get; set; }
        byte? U38 { get; set; }
        byte? U39 { get; set; }
        byte? U40 { get; set; }
    }
}