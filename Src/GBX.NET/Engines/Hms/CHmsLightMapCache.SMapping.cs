namespace GBX.NET.Engines.Hms;

public partial class CHmsLightMapCache
{
    public partial class SMapping
    {
        private int version;
        public int Version { get => version; set => version = value; }

        private int u01;
        public int U01 { get => u01; set => u01 = value; }

        private int u02;
        public int U02 { get => u02; set => u02 = value; }

        private int u03;
        public int U03 { get => u03; set => u03 = value; }

        private float u04;
        public float U04 { get => u04; set => u04 = value; }

        private float u05;
        public float U05 { get => u05; set => u05 = value; }

        private float u06;
        public float U06 { get => u06; set => u06 = value; }

        private float u07;
        public float U07 { get => u07; set => u07 = value; }

        private float u08;
        public float U08 { get => u08; set => u08 = value; }

        private float u09;
        public float U09 { get => u09; set => u09 = value; }

        private int u10 = 1;
        public int U10 { get => u10; set => u10 = value; }

        private int count;

#if NET9_0_OR_GREATER
        private readonly Lock ZlibData1Lock = new();
#else
        private readonly object ZlibData1Lock = new();
#endif
        private ZlibData? zlibData1;
        private float[]? zlibData1Decompressed;
        public ZlibData? ZlibData1 { get => zlibData1; set => zlibData1 = value; }
        public float[]? ZlibData1Decompressed
        {
            get
            {
                if (zlibData1Decompressed is not null) return zlibData1Decompressed;
                if (zlibData1 is null || zlibData1.Parsed) return zlibData1Decompressed;

                lock (ZlibData1Lock)
                {
                    if (zlibData1Decompressed is not null) return zlibData1Decompressed;
                    using var r = zlibData1.OpenDecompressedReader();
                    zlibData1Decompressed = r.ReadArray<float>(count);
                    zlibData1.Parsed = true;
                    return zlibData1Decompressed;
                }
            }
            set
            {
                lock (ZlibData1Lock)
                {
                    zlibData1Decompressed = value;
                }
            }
        }

#if NET9_0_OR_GREATER
        private readonly Lock ZlibData2Lock = new();
#else
        private readonly object ZlibData2Lock = new();
#endif
        private ZlibData? zlibData2;
        private short[]? zlibData2Decompressed1;
        private short[]? zlibData2Decompressed2;
        private short[]? zlibData2Decompressed3;
        public ZlibData? ZlibData2 { get => zlibData2; set => zlibData2 = value; }
        public short[]? ZlibData2Decompressed1
        {
            get
            {
                if (zlibData2Decompressed1 is not null) return zlibData2Decompressed1;
                if (zlibData2 is null || zlibData2.Parsed) return zlibData2Decompressed1;

                lock (ZlibData2Lock)
                {
                    if (zlibData2Decompressed1 is not null) return zlibData2Decompressed1;
                    using var r = zlibData2.OpenDecompressedReader();
                    zlibData2Decompressed1 = r.ReadArray<short>(count);
                    zlibData2Decompressed2 = r.ReadArray<short>(count);
                    zlibData2Decompressed3 = r.ReadArray<short>(count);
                    zlibData2.Parsed = true;
                    return zlibData2Decompressed1;
                }
            }
            set
            {
                lock (ZlibData2Lock)
                {
                    zlibData2Decompressed1 = value;
                }
            }
        }
        public short[]? ZlibData2Decompressed2
        {
            get
            {
                if (zlibData2Decompressed2 is not null) return zlibData2Decompressed2;
                if (zlibData2 is null || zlibData2.Parsed) return zlibData2Decompressed2;

                lock (ZlibData2Lock)
                {
                    if (zlibData2Decompressed2 is not null) return zlibData2Decompressed2;
                    using var r = zlibData2.OpenDecompressedReader();
                    zlibData2Decompressed1 = r.ReadArray<short>(count);
                    zlibData2Decompressed2 = r.ReadArray<short>(count);
                    zlibData2Decompressed3 = r.ReadArray<short>(count);
                    zlibData2.Parsed = true;
                    return zlibData2Decompressed2;
                }
            }
            set
            {
                lock (ZlibData2Lock)
                {
                    zlibData2Decompressed2 = value;
                }
            }
        }
        public short[]? ZlibData2Decompressed3
        {
            get
            {
                if (zlibData2Decompressed3 is not null) return zlibData2Decompressed3;
                if (zlibData2 is null || zlibData2.Parsed) return zlibData2Decompressed3;

                lock (ZlibData2Lock)
                {
                    if (zlibData2Decompressed3 is not null) return zlibData2Decompressed3;
                    using var r = zlibData2.OpenDecompressedReader();
                    zlibData2Decompressed1 = r.ReadArray<short>(count);
                    zlibData2Decompressed2 = r.ReadArray<short>(count);
                    zlibData2Decompressed3 = r.ReadArray<short>(count);
                    zlibData2.Parsed = true;
                    return zlibData2Decompressed3;
                }
            }
            set
            {
                lock (ZlibData2Lock)
                {
                    zlibData2Decompressed3 = value;
                }
            }
        }

#if NET9_0_OR_GREATER
        private readonly Lock ZlibData3Lock = new();
#else
        private readonly object ZlibData3Lock = new();
#endif
        private ZlibData? zlibData3;
        private short[]? zlibData3Decompressed;
        public ZlibData? ZlibData3 { get => zlibData3; set => zlibData3 = value; }
        public short[]? ZlibData3Decompressed
        {
            get
            {
                if (zlibData3Decompressed is not null) return zlibData3Decompressed;
                if (zlibData3 is null || zlibData3.Parsed) return zlibData3Decompressed;

                lock (ZlibData3Lock)
                {
                    if (zlibData3Decompressed is not null) return zlibData3Decompressed;
                    using var r = zlibData3.OpenDecompressedReader();
                    zlibData3Decompressed = r.ReadArray<short>(count * 2);
                    zlibData3.Parsed = true;
                    return zlibData3Decompressed;
                }
            }
            set
            {
                lock (ZlibData3Lock)
                {
                    zlibData3Decompressed = value;
                }
            }
        }

#if NET9_0_OR_GREATER
        private readonly Lock ZlibData4Lock = new();
#else
        private readonly object ZlibData4Lock = new();
#endif
        private ZlibData? zlibData4;
        private short[]? zlibData4Decompressed;
        public ZlibData? ZlibData4 { get => zlibData4; set => zlibData4 = value; }
        public short[]? ZlibData4Decompressed
        {
            get
            {
                if (zlibData4Decompressed is not null) return zlibData4Decompressed;
                if (zlibData4 is null || zlibData4.Parsed) return zlibData4Decompressed;

                lock (ZlibData4Lock)
                {
                    if (zlibData4Decompressed is not null) return zlibData4Decompressed;
                    using var r = zlibData4.OpenDecompressedReader();
                    zlibData4Decompressed = r.ReadArray<short>(count * 2);
                    zlibData4.Parsed = true;
                    return zlibData4Decompressed;
                }
            }
            set
            {
                lock (ZlibData4Lock)
                {
                    zlibData4Decompressed = value;
                }
            }
        }

#if NET9_0_OR_GREATER
        private readonly Lock ZlibData5Lock = new();
#else
        private readonly object ZlibData5Lock = new();
#endif
        private int u11;
        private ZlibData? zlibData5;
        private int? zlibData5Decompressed1;
        private short[]? zlibData5Decompressed2;
        private short[]? zlibData5Decompressed3;
        private short[]? zlibData5Decompressed4;
        public int U11 { get => u11; set => u11 = value; }
        public ZlibData? ZlibData5 { get => zlibData5; set => zlibData5 = value; }
        public int? ZlibData5Decompressed1
        {
            get
            {
                if (zlibData5Decompressed1 is not null) return zlibData5Decompressed1;
                if (zlibData5 is null || zlibData5.Parsed) return zlibData5Decompressed1;

                lock (ZlibData5Lock)
                {
                    if (zlibData5Decompressed1 is not null) return zlibData5Decompressed1;
                    using var r = zlibData5.OpenDecompressedReader();
                    zlibData5Decompressed1 = r.ReadInt32();
                    zlibData5Decompressed2 = r.ReadArray<short>(count * 2);
                    zlibData5Decompressed3 = r.ReadArray<short>(count * 2);
                    zlibData5Decompressed4 = r.ReadArray<short>(count);
                    zlibData5.Parsed = true;
                    return zlibData5Decompressed1;
                }
            }
            set
            {
                lock (ZlibData5Lock)
                {
                    zlibData5Decompressed1 = value;
                }
            }
        }
        public short[]? ZlibData5Decompressed2
        {
            get
            {
                if (zlibData5Decompressed2 is not null) return zlibData5Decompressed2;
                if (zlibData5 is null || zlibData5.Parsed) return zlibData5Decompressed2;

                lock (ZlibData5Lock)
                {
                    if (zlibData5Decompressed2 is not null) return zlibData5Decompressed2;
                    using var r = zlibData5.OpenDecompressedReader();
                    zlibData5Decompressed1 = r.ReadInt32();
                    zlibData5Decompressed2 = r.ReadArray<short>(count * 2);
                    zlibData5Decompressed3 = r.ReadArray<short>(count * 2);
                    zlibData5Decompressed4 = r.ReadArray<short>(count);
                    zlibData5.Parsed = true;
                    return zlibData5Decompressed2;
                }
            }
            set
            {
                lock (ZlibData5Lock)
                {
                    zlibData5Decompressed2 = value;
                }
            }
        }
        public short[]? ZlibData5Decompressed3
        {
            get
            {
                if (zlibData5Decompressed3 is not null) return zlibData5Decompressed3;
                if (zlibData5 is null || zlibData5.Parsed) return zlibData5Decompressed3;

                lock (ZlibData5Lock)
                {
                    if (zlibData5Decompressed3 is not null) return zlibData5Decompressed3;
                    using var r = zlibData5.OpenDecompressedReader();
                    zlibData5Decompressed1 = r.ReadInt32();
                    zlibData5Decompressed2 = r.ReadArray<short>(count * 2);
                    zlibData5Decompressed3 = r.ReadArray<short>(count * 2);
                    zlibData5Decompressed4 = r.ReadArray<short>(count);
                    zlibData5.Parsed = true;
                    return zlibData5Decompressed3;
                }
            }
            set
            {
                lock (ZlibData5Lock)
                {
                    zlibData5Decompressed3 = value;
                }
            }
        }

#if NET9_0_OR_GREATER
        private readonly Lock ZlibData6Lock = new();
#else
        private readonly object ZlibData6Lock = new();
#endif
        private ZlibData? zlibData6;
        private byte[][]? zlibData6Decompressed;
        public ZlibData? ZlibData6 { get => zlibData6; set => zlibData6 = value; }
        public byte[][]? ZlibData6Decompressed
        {
            get
            {
                if (zlibData6Decompressed is not null) return zlibData6Decompressed;
                if (zlibData6 is null || zlibData6.Parsed) return zlibData6Decompressed;

                lock (ZlibData6Lock)
                {
                    if (zlibData6Decompressed is not null) return zlibData6Decompressed;
                    using var r = zlibData6.OpenDecompressedReader();
                    var arrayCount = 1;
                    if (version >= 5)
                    {
                        arrayCount = r.ReadInt32();
                    }
                    zlibData6Decompressed = new byte[arrayCount][];
                    for (var i = 0; i < arrayCount; i++)
                    {
                        zlibData6Decompressed[i] = r.ReadData();
                    }
                    zlibData6.Parsed = true;
                    return zlibData6Decompressed;
                }
            }
            set
            {
                lock (ZlibData6Lock)
                {
                    zlibData6Decompressed = value;
                }
            }
        }

        public void ReadWrite(GbxReaderWriter rw, int v = 0)
        {
            rw.Int32(ref version);

            if (version >= 8)
            {
                rw.Int32(ref u01);
            }

            rw.Int32(ref u02);
            rw.Int32(ref u03);
            rw.Single(ref u04);
            rw.Single(ref u05);
            rw.Single(ref u06);
            rw.Single(ref u07);
            rw.Single(ref u08);
            rw.Single(ref u09);

            if (version >= 6)
            {
                rw.Int32(ref u10);
            }

            // for write, it picks one of the decompressed arrays for count
            // during write, there should be a constraint that all decompressed arrays must have the same length
            count = rw.Int32(zlibData2Decompressed1?.Length ?? 0);

            if (version >= 1)
            {
                rw.ZlibData(ref zlibData1, rw =>
                {
                    rw.Array<float>(ref zlibData1Decompressed, count);
                }, lazyLoad: true);
            }

            rw.ZlibData(ref zlibData2, rw =>
            {
                rw.Array<short>(ref zlibData2Decompressed1, count);
                rw.Array<short>(ref zlibData2Decompressed2, count);
                rw.Array<short>(ref zlibData2Decompressed3, count);
            }, lazyLoad: true);

            rw.ZlibData(ref zlibData3, rw =>
            {
                // GmNat16_2 array
                rw.Array<short>(ref zlibData3Decompressed, count * 2);
            }, lazyLoad: true);

            rw.ZlibData(ref zlibData4, rw =>
            {
                // GmNat16_2 array
                rw.Array<short>(ref zlibData4Decompressed, count * 2);
            }, lazyLoad: true);

            if (version >= 4)
            {
                rw.Int32(ref u11);
                if (u11 != 0)
                {
                    rw.ZlibData(ref zlibData5, rw =>
                    {
                        rw.Int32(ref zlibData5Decompressed1);

                        // GmNat16_2 array
                        rw.Array<short>(ref zlibData5Decompressed2, count * 2);

                        // GmNat16_2 array
                        rw.Array<short>(ref zlibData5Decompressed3, count * 2);

                        rw.Array<short>(ref zlibData5Decompressed4, count);
                    }, lazyLoad: true);
                }
            }

            if (version >= 2)
            {
                rw.ZlibData(ref zlibData6, rw =>
                {
                    var arrayCount = zlibData6Decompressed?.Length ?? 1;

                    if (version >= 5)
                    {
                        rw.Int32(ref arrayCount);
                    }

                    if (rw.Reader is not null)
                    {
                        zlibData6Decompressed = new byte[arrayCount][];

                        for (var i = 0; i < arrayCount; i++)
                        {
                            zlibData6Decompressed[i] = rw.Reader.ReadData();
                        }
                    }

                    if (rw.Writer is not null)
                    {
                        for (var i = 0; i < arrayCount; i++)
                        {
                            rw.Writer.WriteData(zlibData6Decompressed?[i] ?? []);
                        }
                    }
                }, lazyLoad: true);
            }
        }
    }
}
