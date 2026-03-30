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

    public sealed class Sample : CGameGhost.Data.Sample, ISampleRawData
    {
        private ushort speedForward;
        private ushort speedSideward;
        private ushort rpm;
        private ushort flWheelRotation;
        private ushort frWheelRotation;
        private ushort rrWheelRotation;
        private ushort rlWheelRotation;
        private byte steer;
        private byte gas;
        private byte brake;
        private byte u11;
        private byte u13;
        private byte u14;
        private byte turboStrength;
        private byte steerFront;
        private byte flDampenLen;
        private byte frDampenLen;
        private byte rrDampenLen;
        private byte rlDampenLen;
        private byte? dirtBlend;
        private byte? u33;
        private byte? u34;
        private byte? u35;
        private byte? u36;
        private byte? u37;
        private byte? u38;
        private byte? u40;
        private (Vec3, Quat, byte)[]? u35_1;

        ushort ISampleRawData.SpeedForward { get => speedForward; set => speedForward = value; }
        ushort ISampleRawData.SpeedSideward { get => speedSideward; set => speedSideward = value; }
        ushort ISampleRawData.RPM { get => rpm; set => rpm = value; }
        ushort ISampleRawData.FLWheelRotation { get => flWheelRotation; set => flWheelRotation = value; }
        ushort ISampleRawData.FRWheelRotation { get => frWheelRotation; set => frWheelRotation = value; }
        ushort ISampleRawData.RRWheelRotation { get => rrWheelRotation; set => rrWheelRotation = value; }
        ushort ISampleRawData.RLWheelRotation { get => rlWheelRotation; set => rlWheelRotation = value; }
        byte ISampleRawData.Steer { get => steer; set => steer = value; }
        byte ISampleRawData.Gas { get => gas; set => gas = value; }
        byte ISampleRawData.Brake { get => brake; set => brake = value; }
        byte ISampleRawData.U11 { get => u11; set => u11 = value; }
        byte ISampleRawData.U13 { get => u13; set => u13 = value; }
        byte ISampleRawData.U14 { get => u14; set => u14 = value; }
        byte ISampleRawData.TurboStrength { get => turboStrength; set => turboStrength = value; }
        byte ISampleRawData.SteerFront { get => steerFront; set => steerFront = value; }
        byte ISampleRawData.FLDampenLen { get => flDampenLen; set => flDampenLen = value; }
        byte ISampleRawData.FRDampenLen { get => frDampenLen; set => frDampenLen = value; }
        byte ISampleRawData.RRDampenLen { get => rrDampenLen; set => rrDampenLen = value; }
        byte ISampleRawData.RLDampenLen { get => rlDampenLen; set => rlDampenLen = value; }
        byte? ISampleRawData.DirtBlend { get => dirtBlend; set => dirtBlend = value; }
        byte? ISampleRawData.U33 { get => u33; set => u33 = value; }
        byte? ISampleRawData.U34 { get => u34; set => u34 = value; }
        byte? ISampleRawData.U35 { get => u35; set => u35 = value; }
        byte? ISampleRawData.U36 { get => u36; set => u36 = value; }
        byte? ISampleRawData.U37 { get => u37; set => u37 = value; }
        byte? ISampleRawData.U38 { get => u38; set => u38 = value; }
        byte? ISampleRawData.U40 { get => u40; set => u40 = value; }

        public float SpeedForward
        {
            get => (speedForward / 65535f * 11000 - 1000) * 3.6f;
            set => speedForward = (ushort)AdditionalMath.Clamp(Math.Round(((value / 3.6f) + 1000) / 11000f * 65535f), 0, 65535);
        }

        public float SpeedSideward
        {
            get => speedSideward / 65535f * 2000 - 1000;
            set => speedSideward = (ushort)AdditionalMath.Clamp(Math.Round((value + 1000) / 2000f * 65535f), 0, 65535);
        }

        public float RPM
        {
            get => rpm / 65535f * 30000;
            set => rpm = (ushort)AdditionalMath.Clamp(Math.Round(value / 30000f * 65535f), 0, 65535);
        }

        public float FLWheelRotation
        {
            get => flWheelRotation / 65535f * 1608.495f;
            set => flWheelRotation = (ushort)AdditionalMath.Clamp(Math.Round(value / 1608.495f * 65535f), 0, 65535);
        }

        public float FRWheelRotation
        {
            get => frWheelRotation / 65535f * 1608.495f;
            set => frWheelRotation = (ushort)AdditionalMath.Clamp(Math.Round(value / 1608.495f * 65535f), 0, 65535);
        }

        public float RRWheelRotation
        {
            get => rrWheelRotation / 65535f * 1608.495f;
            set => rrWheelRotation = (ushort)AdditionalMath.Clamp(Math.Round(value / 1608.495f * 65535f), 0, 65535);
        }

        public float RLWheelRotation
        {
            get => rlWheelRotation / 65535f * 1608.495f;
            set => rlWheelRotation = (ushort)AdditionalMath.Clamp(Math.Round(value / 1608.495f * 65535f), 0, 65535);
        }

        public float Steer
        {
            get => steer / 255f * 2 - 1;
            set => steer = (byte)AdditionalMath.Clamp(Math.Round((value + 1f) / 2f * 255f), 0, 255);
        }

        public float Gas
        {
            get => gas / 255f;
            set => gas = (byte)AdditionalMath.Clamp(Math.Round(value * 255f), 0, 255);
        }

        public float Brake
        {
            get => brake / 255f;
            set => brake = (byte)AdditionalMath.Clamp(Math.Round(value * 255f), 0, 255);
        }

#if NET5_0_OR_GREATER
        public float U09_U10_1 => Math.Clamp(Gas + Brake, 0f, 1f);
#else
        public float U09_U10_1 => (float)AdditionalMath.Clamp(Gas + Brake, 0f, 1f);
#endif
        public bool U09_U10_2 => Gas < Brake;

        public float U11
        {
            get => u11 / 255f;
            set => u11 = (byte)AdditionalMath.Clamp(Math.Round(value * 255f), 0, 255);
        }

        public byte U12 { get; set; }

        public float U13
        {
            get => u13 / 255f * 2 - 1;
            set => u13 = (byte)AdditionalMath.Clamp(Math.Round((value + 1f) / 2f * 255f), 0, 255);
        }

        public float U14
        {
            get => u14 / 255f * 2 - 1;
            set => u14 = (byte)AdditionalMath.Clamp(Math.Round((value + 1f) / 2f * 255f), 0, 255);
        }

        public float TurboStrength
        {
            get => turboStrength / 255f;
            set => turboStrength = (byte)AdditionalMath.Clamp(Math.Round(value * 255f), 0, 255);
        }

        public float SteerFront
        {
#if NET6_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
            get => steerFront / 255f * MathF.PI * 2 - MathF.PI;
            set => steerFront = (byte)AdditionalMath.Clamp(Math.Round((value + MathF.PI) / (MathF.PI * 2) * 255f), 0, 255);
#else
            get => steerFront / 255f * (float)Math.PI * 2 - (float)Math.PI;
            set => steerFront = (byte)AdditionalMath.Clamp(Math.Round((value + Math.PI) / (Math.PI * 2) * 255f), 0, 255);
#endif
        }

        public float FLDampenLen
        {
            get => flDampenLen / 255f * 4 - 2;
            set => flDampenLen = (byte)AdditionalMath.Clamp(Math.Round((value + 2f) / 4f * 255f), 0, 255);
        }
        public CPlugSurface.MaterialId FLGroundContactMaterial { get; set; }

        public float FRDampenLen
        {
            get => frDampenLen / 255f * 4 - 2;
            set => frDampenLen = (byte)AdditionalMath.Clamp(Math.Round((value + 2f) / 4f * 255f), 0, 255);
        }
        public CPlugSurface.MaterialId FRGroundContactMaterial { get; set; }

        public float RRDampenLen
        {
            get => rrDampenLen / 255f * 4 - 2;
            set => rrDampenLen = (byte)AdditionalMath.Clamp(Math.Round((value + 2f) / 4f * 255f), 0, 255);
        }
        public CPlugSurface.MaterialId RRGroundContactMaterial { get; set; }

        public float RLDampenLen
        {
            get => rlDampenLen / 255f * 4 - 2;
            set => rlDampenLen = (byte)AdditionalMath.Clamp(Math.Round((value + 2f) / 4f * 255f), 0, 255);
        }
        public CPlugSurface.MaterialId RLGroundContactMaterial { get; set; }

        public byte U25 { get; set; }
        public byte U25_1 => (byte)(U25 & 7);
        public byte Horn => (byte)((U25 >> 3) & 3);
        public byte U25_3 => (byte)((U25 >> 5) & 3);
        public bool U25_4 => (U25 >> 7) != 0;

        public byte U26 { get; set; }
        public bool FLIsSliding
        {
            get => (U26 & 0x40) != 0;
            set { if (value) U26 |= 0x40; else U26 &= 0xBF; }
        }
        public bool FLOnGround => (U26 & 0x80) != 0;

        public byte U27 { get; set; }
        public bool FRIsSliding
        {
            get => (U27 & 0x01) != 0;
            set { if (value) U27 |= 0x01; else U27 &= 0xFE; }
        }
        public bool FROnGround => (U27 & 0x02) != 0;

        public bool RRIsSliding
        {
            get => (U27 & 0x04) != 0;
            set { if (value) U27 |= 0x04; else U27 &= 0xFB; }
        }
        public bool RROnGround => (U27 & 0x08) != 0;

        public bool RLIsSliding
        {
            get => (U27 & 0x10) != 0;
            set { if (value) U27 |= 0x10; else U27 &= 0xEF; }
        }
        public bool RLOnGround => (U27 & 0x20) != 0;
        public bool U27_7 => (U27 & 0x40) != 0;
        public bool U27_8 => (U27 & 0x80) != 0;

        public float? DirtBlend
        {
            get => dirtBlend.HasValue ? dirtBlend.Value / 255f : null;
            set => dirtBlend = value.HasValue ? (byte)AdditionalMath.Clamp(Math.Round(value.Value * 255f), 0, 255) : null;
        }

        public Vec4? U33
        {
            get => u33.HasValue ? new Vec4((u33.Value & 3) / 3f, ((u33.Value >> 2) & 3) / 3f, ((u33.Value >> 4) & 3) / 3f, ((u33.Value >> 6) & 3) / 3f) : null;
            set => UpdateVec4Flag(ref u33, value);
        }

        public Vec4? U34
        {
            get => u34.HasValue ? new Vec4((u34.Value & 3) / 3f, ((u34.Value >> 2) & 3) / 3f, ((u34.Value >> 4) & 3) / 3f, ((u34.Value >> 6) & 3) / 3f) : null;
            set => UpdateVec4Flag(ref u34, value);
        }

        public float? U35
        {
            get => u35.HasValue ? (u35.Value & 3) / 3f : null;
            set
            {
                if (!value.HasValue)
                {
                    if (u35.HasValue) u35 = (byte)(u35.Value & ~3);
                }
                else
                {
                    u35 = (byte)((u35.GetValueOrDefault() & ~3) | (byte)AdditionalMath.Clamp(Math.Round(value.Value * 3f), 0, 3));
                }
            }
        }

        public (Vec3, Quat, byte)[]? U35_1
        {
            get => u35_1;
            set
            {
                u35_1 = value;
                if (value != null)
                {
                    var count = (byte)AdditionalMath.Clamp(value.Length, 0, 7);
                    u35 = (byte)((u35.GetValueOrDefault() & ~(7 << 2)) | (count << 2));
                }
            }
        }

        public bool? U36_1 { get => GetBit(u36, 0); set => UpdateBit(ref u36, 0, value); }
        public bool? U36_2 { get => GetBit(u36, 1); set => UpdateBit(ref u36, 1, value); }
        public bool? U36_3 { get => GetBit(u36, 2); set => UpdateBit(ref u36, 2, value); }
        public bool? U36_4 { get => GetBit(u36, 3); set => UpdateBit(ref u36, 3, value); }
        public bool? U36_5 { get => GetBit(u36, 4); set => UpdateBit(ref u36, 4, value); }

        public float? U37_1
        {
            get => u37.HasValue ? (u37.Value & 3) / 3f : null;
            set
            {
                if (!value.HasValue) { if (u37.HasValue) u37 = (byte)(u37.Value & ~3); }
                else u37 = (byte)((u37.GetValueOrDefault() & ~3) | (byte)AdditionalMath.Clamp(Math.Round(value.Value * 3f), 0, 3));
            }
        }

        public bool? U37_2 { get => GetBit(u37, 3); set => UpdateBit(ref u37, 3, value); }

        public int? U37_3
        {
            get => u37.HasValue ? u37.Value >> 4 : null;
            set
            {
                if (!value.HasValue) { if (u37.HasValue) u37 = (byte)(u37.Value & 0x0F); }
                else u37 = (byte)((u37.GetValueOrDefault() & 0x0F) | ((value.Value & 0x0F) << 4));
            }
        }

        public float? U38
        {
            get => u38.HasValue ? u38.Value / 255f * 5 : null;
            set => u38 = value.HasValue ? (byte)AdditionalMath.Clamp(Math.Round(value.Value / 5f * 255f), 0, 255) : null;
        }

        public byte? U39 { get; set; }

        public float? U40
        {
            get => u40.HasValue ? u40.Value / 255f : null;
            set => u40 = value.HasValue ? (byte)AdditionalMath.Clamp(Math.Round(value.Value * 255f), 0, 255) : null;
        }

        internal Sample(TimeInt32 time, byte[] data) : base(time, data)
        {
        }

        internal override void Read(GbxReader r, int version)
        {
            if (version < 7)
            {
                throw new VersionNotSupportedException(version);
            }

            // CHmsDynaReplayItem::RestoreDynaItemState

            // HmsStateVersion == 0 (EHmsDynaItemSaveStateVersion_TmNetworkAfter260205)
            // Position 9-byte Vec3
            // Rotation = r.ReadQuat6();

            // HmsStateVersion == 1 (EHmsDynaItemSaveStateVersion_TmReplayAfter260205)
            Position = version == 13 ? r.ReadVec3_9() : r.ReadVec3();
            Rotation = r.ReadQuat6();

            if (version == 13)
            {
                // SVehicleSimpleNetState::ToVehicle
                FLGroundContactMaterial = CPlugSurface.MaterialId.Asphalt;
                FRGroundContactMaterial = CPlugSurface.MaterialId.Asphalt;
                RRGroundContactMaterial = CPlugSurface.MaterialId.Asphalt;
                RLGroundContactMaterial = CPlugSurface.MaterialId.Asphalt;

                var netData = r.ReadUInt16();

                Brake = netData >> 1 & 1;

                var isSliding = (netData >> 2 & 1) != 0;
                FLIsSliding = isSliding;
                FRIsSliding = isSliding;
                RRIsSliding = isSliding;
                RLIsSliding = isSliding;

                // Calculate RPM
                var min = 100f; // guessed cuz its stored by vehicle
                var max = 11000f; // guessed cuz its stored by vehicle
                var ratio = min / max; // min/max rpm i guess
#if NET6_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
                var powRatio = MathF.Pow(ratio, 3.0f);
#else
                var powRatio = Math.Pow(ratio, 3.0f);
#endif
                var normalizedValue = (netData >> 9) / 127.0f;
#if NET6_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
                var rpmValue = MathF.Pow(normalizedValue * (1.0f - powRatio) + powRatio, 0.3f);
#else
                var rpmValue = Math.Pow(normalizedValue * (1.0f - powRatio) + powRatio, 0.3f);
#endif
                rpmValue *= max;
                RPM = (float)rpmValue;
                return;
            }

            Velocity = r.ReadVec3_4();
            AngularVelocity = r.ReadVec3_4();

            // CSceneVehicleVis_RestoreStaticState
            if (version >= 7)
            {
                SpeedForward = (r.ReadUInt16() / 65535f * 11000 - 1000) * 3.6f;
                SpeedSideward = r.ReadUInt16() / 65535f * 2000 - 1000;
                RPM = r.ReadUInt16() / 65535f * 30000;
                FLWheelRotation = r.ReadUInt16() / 65535f * 1608.495f;
                FRWheelRotation = r.ReadUInt16() / 65535f * 1608.495f;
                RRWheelRotation = r.ReadUInt16() / 65535f * 1608.495f;
                RLWheelRotation = r.ReadUInt16() / 65535f * 1608.495f;
                Steer = r.ReadByte() / 255f * 2 - 1;
                Gas = r.ReadByte() / 255f;
                Brake = r.ReadByte() / 255f;

                U11 = r.ReadByte() / 255f;

                if (version >= 8)
                {
                    U12 = r.ReadByte(); // it should be always 0 but sometimes it isnt
                }

                U13 = r.ReadByte() / 255f * 2 - 1;
                U14 = r.ReadByte() / 255f * 2 - 1;
                TurboStrength = r.ReadByte() / 255f;
#if NET6_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
                SteerFront = r.ReadByte() / 255f * MathF.PI * 2 - MathF.PI;
#else
                SteerFront = r.ReadByte() / 255f * (float)Math.PI * 2 - (float)Math.PI;
#endif

                FLDampenLen = r.ReadByte() / 255f * 4 - 2;
                FLGroundContactMaterial = (CPlugSurface.MaterialId)r.ReadByte();
                // + condition "in water"

                FRDampenLen = r.ReadByte() / 255f * 4 - 2;
                FRGroundContactMaterial = (CPlugSurface.MaterialId)r.ReadByte();
                // + condition "in water"

                RRDampenLen = r.ReadByte() / 255f * 4 - 2; // RRDampenLenByte?
                RRGroundContactMaterial = (CPlugSurface.MaterialId)r.ReadByte();
                // + condition "in water"

                RLDampenLen = r.ReadByte() / 255f * 4 - 2; // RLDampenLenByte?
                RLGroundContactMaterial = (CPlugSurface.MaterialId)r.ReadByte();
                // + condition "in water"

                if (version >= 8)
                {
                    U25 = r.ReadByte();
                    U26 = r.ReadByte();
                    U27 = r.ReadByte(); // similar to U26, some form of flags

                    if (version >= 9)
                    {
                        DirtBlend = r.ReadByte() / 255f;

                        if (version >= 10)
                        {
                            var unavailableVal2 = r.ReadUInt32();

                            if (unavailableVal2 != 0)
                            {
                                throw new Exception();
                            }

                            // damage amount

                            var u33 = r.ReadByte();
                            U33 = new Vec4((u33 & 3) / 3f, ((u33 >> 2) & 3) / 3f, ((u33 >> 4) & 3) / 3f, ((u33 >> 6) & 3) / 3f);

                            var u34 = r.ReadByte();
                            U34 = new Vec4((u34 & 3) / 3f, ((u34 >> 2) & 3) / 3f, ((u34 >> 4) & 3) / 3f, ((u34 >> 6) & 3) / 3f);

                            var u35 = r.ReadByte();
                            U35 = (u35 & 3) / 3f;

                            if (version >= 11)
                            {
                                if (version >= 14)
                                {
                                    var u36 = r.ReadByte();
                                    U36_1 = Convert.ToBoolean(u36 & 1);
                                    U36_2 = Convert.ToBoolean(u36 >> 1 & 1);
                                    U36_3 = Convert.ToBoolean(u36 >> 2 & 1);
                                    U36_4 = Convert.ToBoolean(u36 >> 3 & 1);
                                    U36_5 = Convert.ToBoolean(u36 >> 4 & 1);

                                    var u37 = r.ReadByte();
                                    U37_1 = (u37 & 3) / 3f;
                                    U37_2 = Convert.ToBoolean(u37 >> 3 & 1);
                                    U37_3 = u37 >> 4;

                                    if (version >= 15)
                                    {
                                        U38 = r.ReadByte() / 255f * 5;

                                        if (version >= 16)
                                        {
                                            U39 = r.ReadByte();
                                            U40 = r.ReadByte() / 255f;
                                        }
                                    }
                                }

                                // count is broken in specific cases like the last sample of a ghost
                                var count = u35 >> 2 & 7;

                                if (version == 11 && count > 4)
                                {
                                    count = 4;
                                }

                                U35_1 = new (Vec3, Quat, byte)[count];

                                for (var i = 0; i < count; i++)
                                {
                                    U35_1[i] = (r.ReadVec3(), r.ReadQuat6(), r.ReadByte());
                                }
                            }
                        }
                    }
                }
            }

            if (r.BaseStream.Position != r.BaseStream.Length)
            {
                throw new Exception("Not all bytes were read");
            }
        }

        internal override void Write(GbxWriter w, int version)
        {
            if (version < 7)
            {
                throw new VersionNotSupportedException(version);
            }

            if (version == 13)
            {
                w.WriteVec3_9(Position);
                w.WriteQuat6(Rotation);

                ushort netData = 0;

                if (Brake >= 0.5f) netData |= 1 << 1;

                // FLIsSliding handles standard sliding bit 
                if (FLIsSliding) netData |= 1 << 2;

                var min = 100f;
                var max = 11000f;
                var ratio = min / max;
#if NET6_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
                var powRatio = MathF.Pow(ratio, 3.0f);
                var rpmValue = Math.Clamp(RPM, min, max) / max;
#else
                var powRatio = Math.Pow(ratio, 3.0f);
                var rpmValue = AdditionalMath.Clamp(RPM, min, max) / max;
#endif

#if NET6_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
                var normalizedValue = (MathF.Pow(rpmValue, 1f / 0.3f) - powRatio) / (1.0f - powRatio);
#else
                var normalizedValue = (Math.Pow(rpmValue, 1f/0.3f) - powRatio) / (1.0f - powRatio);
#endif
                var rpmBits = (ushort)AdditionalMath.Clamp(Math.Round(normalizedValue * 127f), 0, 127);
                netData |= (ushort)(rpmBits << 9);

                w.Write(netData);
                return;
            }

            w.Write(Position);
            w.WriteQuat6(Rotation);

            w.WriteVec3_4(Velocity);
            w.WriteVec3_4(AngularVelocity);

            if (version >= 7)
            {
                w.Write(speedForward);
                w.Write(speedSideward);
                w.Write(rpm);
                w.Write(flWheelRotation);
                w.Write(frWheelRotation);
                w.Write(rrWheelRotation);
                w.Write(rlWheelRotation);
                w.Write(steer);
                w.Write(gas);
                w.Write(brake);
                w.Write(u11);

                if (version >= 8)
                {
                    w.Write(U12);
                }

                w.Write(u13);
                w.Write(u14);
                w.Write(turboStrength);
                w.Write(steerFront);
                w.Write(flDampenLen);
                w.Write((byte)FLGroundContactMaterial);
                w.Write(frDampenLen);
                w.Write((byte)FRGroundContactMaterial);
                w.Write(rrDampenLen);
                w.Write((byte)RRGroundContactMaterial);
                w.Write(rlDampenLen);
                w.Write((byte)RLGroundContactMaterial);

                if (version >= 8)
                {
                    w.Write(U25);
                    w.Write(U26);
                    w.Write(U27);

                    if (version >= 9)
                    {
                        w.Write(dirtBlend.GetValueOrDefault());

                        if (version >= 10)
                        {
                            w.Write(0u); // unavailableVal2
                            w.Write(u33.GetValueOrDefault());
                            w.Write(u34.GetValueOrDefault());
                            w.Write(u35.GetValueOrDefault());

                            if (version >= 11)
                            {
                                if (version >= 14)
                                {
                                    w.Write(u36.GetValueOrDefault());
                                    w.Write(u37.GetValueOrDefault());

                                    if (version >= 15)
                                    {
                                        w.Write(u38.GetValueOrDefault());

                                        if (version >= 16)
                                        {
                                            w.Write(U39.GetValueOrDefault());
                                            w.Write(u40.GetValueOrDefault());
                                        }
                                    }
                                }

                                var count = u35.GetValueOrDefault() >> 2 & 7;
                                if (version == 11 && count > 4) count = 4;

                                if (u35_1 != null)
                                {
                                    for (var i = 0; i < count && i < u35_1.Length; i++)
                                    {
                                        w.Write(u35_1[i].Item1);
                                        w.WriteQuat6(u35_1[i].Item2);
                                        w.Write(u35_1[i].Item3);
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        // --- Bitwise Helpers ---

        private static bool? GetBit(byte? flagByte, int bit) => flagByte.HasValue ? (flagByte.Value & (1 << bit)) != 0 : null;
        private static void UpdateBit(ref byte? flagByte, int bit, bool? value)
        {
            if (!value.HasValue)
            {
                if (flagByte.HasValue) flagByte = (byte)(flagByte.Value & ~(1 << bit));
            }
            else
            {
                if (value.Value) flagByte = (byte)(flagByte.GetValueOrDefault() | (1 << bit));
                else flagByte = (byte)(flagByte.GetValueOrDefault() & ~(1 << bit));
            }
        }

        private static void UpdateVec4Flag(ref byte? flagByte, Vec4? value)
        {
            if (!value.HasValue) flagByte = null;
            else
            {
                byte b = 0;
                b |= (byte)((int)AdditionalMath.Clamp(Math.Round(value.Value.X * 3f), 0, 3));
                b |= (byte)((int)AdditionalMath.Clamp(Math.Round(value.Value.Y * 3f), 0, 3) << 2);
                b |= (byte)((int)AdditionalMath.Clamp(Math.Round(value.Value.Z * 3f), 0, 3) << 4);
                b |= (byte)((int)AdditionalMath.Clamp(Math.Round(value.Value.W * 3f), 0, 3) << 6);
                flagByte = b;
            }
        }
    }
}