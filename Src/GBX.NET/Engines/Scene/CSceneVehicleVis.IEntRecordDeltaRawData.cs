namespace GBX.NET.Engines.Scene;

public partial class CSceneVehicleVis
{
    public interface IEntRecordDeltaRawData
    {
        ushort SideSpeed { get; set; }
        byte Rpm { get; set; }
        byte FlWheelRot { get; set; }
        byte FlWheelRotCount { get; set; }
        byte FrWheelRot { get; set; }
        byte FrWheelRotCount { get; set; }
        byte RrWheelRot { get; set; }
        byte RrWheelRotCount { get; set; }
        byte RlWheelRot { get; set; }
        byte RlWheelRotCount { get; set; }
        byte Steer { get; set; }
        byte U15 { get; set; }
        byte Brake { get; set; }
        byte TurboTime { get; set; }
        byte FlDampenLen { get; set; }
        CPlugSurface.MaterialId FLGroundContactMaterial { get; set; }
        byte FrDampenLen { get; set; }
        CPlugSurface.MaterialId FRGroundContactMaterial { get; set; }
        byte RrDampenLen { get; set; }
        CPlugSurface.MaterialId RRGroundContactMaterial { get; set; }
        byte RlDampenLen { get; set; }
        CPlugSurface.MaterialId RLGroundContactMaterial { get; set; }
        byte IsTurbo { get; set; }
        byte SlipCoef1 { get; set; }
        byte SlipCoef2 { get; set; }
        byte VehicleState { get; set; }
        byte FlIce { get; set; }
        byte FrIce { get; set; }
        byte RrIce { get; set; }
        byte RlIce { get; set; }
        byte GroundMode { get; set; }
        byte BoosterAirControl { get; set; }
        byte Gear { get; set; }
        byte FlDirt { get; set; }
        byte FrDirt { get; set; }
        byte RrDirt { get; set; }
        byte RlDirt { get; set; }
        byte WetnessValue { get; set; }
        byte SimulationTimeCoef { get; set; }
    }
}