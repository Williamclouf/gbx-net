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

        private ZlibData? zlibData1;
        private float[]? zlibData1Decompressed;
        public ZlibData? ZlibData1 { get => zlibData1; set => zlibData1 = value; }
        public float[]? ZlibData1Decompressed { get => zlibData1Decompressed; set => zlibData1Decompressed = value; }

        private ZlibData? zlibData2;
        private short[] zlibData2Decompressed1 = [];
        private short[] zlibData2Decompressed2 = [];
        private short[] zlibData2Decompressed3 = [];
        public ZlibData? ZlibData2 { get => zlibData2; set => zlibData2 = value; }
        public short[] ZlibData2Decompressed1 { get => zlibData2Decompressed1; set => zlibData2Decompressed1 = value; }
        public short[] ZlibData2Decompressed2 { get => zlibData2Decompressed2; set => zlibData2Decompressed2 = value; }
        public short[] ZlibData2Decompressed3 { get => zlibData2Decompressed3; set => zlibData2Decompressed3 = value; }

        private ZlibData? zlibData3;
        private short[]? zlibData3Decompressed;
        public ZlibData? ZlibData3 { get => zlibData3; set => zlibData3 = value; }
        public short[]? ZlibData3Decompressed { get => zlibData3Decompressed; set => zlibData3Decompressed = value; }

        private ZlibData? zlibData4;
        private short[]? zlibData4Decompressed;
        public ZlibData? ZlibData4 { get => zlibData4; set => zlibData4 = value; }
        public short[]? ZlibData4Decompressed { get => zlibData4Decompressed; set => zlibData4Decompressed = value; }

        private int u11;
        private ZlibData? zlibData5;
        private int? zlibData5Decompressed1;
        private short[]? zlibData5Decompressed2;
        private short[]? zlibData5Decompressed3;
        private short[]? zlibData5Decompressed4;
        public int U11 { get => u11; set => u11 = value; }
        public ZlibData? ZlibData5 { get => zlibData5; set => zlibData5 = value; }
        public int? ZlibData5Decompressed1 { get => zlibData5Decompressed1; set => zlibData5Decompressed1 = value; }
        public short[]? ZlibData5Decompressed2 { get => zlibData5Decompressed2; set => zlibData5Decompressed2 = value; }
        public short[]? ZlibData5Decompressed3 { get => zlibData5Decompressed3; set => zlibData5Decompressed3 = value; }

        private ZlibData? zlibData6;
        private byte[][]? zlibData6Decompressed;
        public ZlibData? ZlibData6 { get => zlibData6; set => zlibData6 = value; }
        public byte[][]? ZlibData6Decompressed { get => zlibData6Decompressed; set => zlibData6Decompressed = value; }

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

            var count = rw.Int32(zlibData2Decompressed1.Length);

            if (version >= 1)
            {
                rw.ZlibData(ref zlibData1, rw =>
                {
                    rw.Array<float>(ref zlibData1Decompressed, count);
                }, lazyLoad: false);
            }

            rw.ZlibData(ref zlibData2, rw =>
            {
                rw.Array<short>(ref zlibData2Decompressed1, count);
                rw.Array<short>(ref zlibData2Decompressed2, count);
                rw.Array<short>(ref zlibData2Decompressed3, count);
            }, lazyLoad: false);

            rw.ZlibData(ref zlibData3, rw =>
            {
                // GmNat16_2 array
                rw.Array<short>(ref zlibData3Decompressed, count * 2);
            }, lazyLoad: false);

            rw.ZlibData(ref zlibData4, rw =>
            {
                // GmNat16_2 array
                rw.Array<short>(ref zlibData4Decompressed, count * 2);
            }, lazyLoad: false);

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
                    }, lazyLoad: false);
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
                }, lazyLoad: false);
            }
        }
    }
}
