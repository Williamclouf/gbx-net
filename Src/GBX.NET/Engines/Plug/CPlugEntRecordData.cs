using GBX.NET.Managers;

namespace GBX.NET.Engines.Plug;

public partial class CPlugEntRecordData : IReadableWritable
{
    private TimeInt32? start;
    private TimeInt32? end;
    private EntRecordDesc[] entRecordDescs = [];
    private NoticeRecordDesc[] noticeRecordDescs = [];
    private List<EntRecordListElem> entList = [];
    private List<NoticeRecordListElem> bulkNoticeList = [];
    private List<CustomModulesDeltaList> customModulesDeltaLists = [];

    private ZlibData? compressedData;
    public ZlibData? CompressedData { get => compressedData; set => compressedData = value; }

    /// <exception cref="ZLibNotDefinedException">Zlib is not defined.</exception>
    public TimeInt32 Start
    {
        get => compressedData?.Exception is not null
            ? throw compressedData.Exception
            : start ?? TimeInt32.Zero;
        set => start = value;
    }

    /// <exception cref="ZLibNotDefinedException">Zlib is not defined.</exception>
    public TimeInt32 End
    {
        get => compressedData?.Exception is not null
            ? throw compressedData.Exception
            : end ?? TimeInt32.Zero;
        set => end = value;
    }

    /// <exception cref="ZLibNotDefinedException">Zlib is not defined.</exception>
    public EntRecordDesc[] EntRecordDescs
    {
        get => compressedData?.Exception is not null
            ? throw compressedData.Exception
            : entRecordDescs ?? [];
        set => entRecordDescs = value;
    }

    /// <exception cref="ZLibNotDefinedException">Zlib is not defined.</exception>
    public NoticeRecordDesc[] NoticeRecordDescs
    {
        get => compressedData?.Exception is not null
            ? throw compressedData.Exception
            : noticeRecordDescs ?? [];
        set => noticeRecordDescs = value;
    }

    /// <exception cref="ZLibNotDefinedException">Zlib is not defined.</exception>
    public List<EntRecordListElem> EntList
    {
        get => compressedData?.Exception is not null
            ? throw compressedData.Exception
            : entList ?? [];
        set => entList = value;
    }

    /// <exception cref="ZLibNotDefinedException">Zlib is not defined.</exception>
    public List<NoticeRecordListElem> BulkNoticeList
    {
        get => compressedData?.Exception is not null
            ? throw compressedData.Exception
            : bulkNoticeList ?? [];
        set => bulkNoticeList = value;
    }

    /// <exception cref="ZLibNotDefinedException">Zlib is not defined.</exception>
    public List<CustomModulesDeltaList> CustomModulesDeltaLists
    {
        get => compressedData?.Exception is not null
            ? throw compressedData.Exception
            : customModulesDeltaLists ?? [];
        set => customModulesDeltaLists = value;
    }

    public void ReadWrite(GbxReaderWriter rw, int v = 0)
    {
        if (v >= 1)
        {
            rw.TimeInt32(ref start);
            rw.TimeInt32(ref end);
        }

        rw.ArrayReadableWritable<EntRecordDesc>(ref entRecordDescs);

        if (v >= 2)
        {
            rw.ArrayReadableWritable<NoticeRecordDesc>(ref noticeRecordDescs!, version: v);
        }

        if (rw.Reader is not null)
        {
            entList = ReadEntList(rw.Reader, v).ToList();

            if (v >= 3)
            {
                bulkNoticeList = ReadBulkNoticeList(rw.Reader).ToList();

                // custom modules
                customModulesDeltaLists = ReadCustomModulesDeltaLists(rw.Reader, v).ToList();
            }
        }

        if (rw.Writer is not null)
        {
            WriteEntList(rw.Writer, v);

            if (v >= 3)
            {
                WriteBulkNoticeList(rw.Writer);
                WriteCustomModulesDeltaLists(rw.Writer, v);
            }
        }
    }

    public partial class Chunk0911F000 : IVersionable
    {
        public int Version { get; set; }

        public override void ReadWrite(CPlugEntRecordData n, GbxReaderWriter rw)
        {
            rw.VersionInt32(this);

            if (Version < 5)
            {
                n.ReadWrite(rw, Version);
                return;
            }

            rw.ZlibData(ref n.compressedData, n, Version);
        }
    }

    private IEnumerable<EntRecordListElem> ReadEntList(GbxReader r, int version)
    {
        var hasNextElem = r.ReadBoolean(asByte: true);

        while (hasNextElem)
        {
            var type = r.ReadInt32();
            var u01 = r.ReadInt32();
            var u02 = r.ReadInt32(); // start?
            var u03 = r.ReadInt32(); // end? ghostLengthFinish ms

            var u04 = version >= 6 ? r.ReadInt32() : u01;

            var desc = entRecordDescs[type];
            List<EntRecordDelta> samples;
            if (version < 11)
            {
                samples = ReadEntRecordDeltas(r, desc).ToList();
            }
            else
            {
                var buffers = ReadEncodedDeltas(r);
                samples = new(buffers.Length);

                var time = TimeInt32.Zero;
                for (var i = 0; i < buffers.Length; i++)
                {
                    var (deltaTime, data) = buffers[i];
                    time += deltaTime;
                    samples.Add(CreateEntRecordDelta(time, data, desc));
                }
            }

            hasNextElem = r.ReadBoolean(asByte: true);

            List<EntRecordDelta2> samples2 = version >= 2 ? ReadEntRecordDeltas2(r).ToList() : [];

            yield return new EntRecordListElem
            {
                Type = type,
                U01 = u01,
                U02 = u02,
                U03 = u03,
                U04 = u04,
                Samples = samples,
                Samples2 = samples2
            };
        }
    }

    private static (TimeInt32 deltaTime, byte[] data)[] ReadEncodedDeltas(GbxReader r)
    {
        var numSamples = r.ReadInt32();

        if (numSamples == 0)
        {
            return [];
        }

        var sampleSize = r.ReadInt32();

        if (numSamples < 0 || sampleSize < 0)
        {
            throw new InvalidDataException($"Invalid header dimensions: {numSamples}x{sampleSize}");
        }

        var samples = new (TimeInt32 deltaTime, byte[] data)[numSamples];
        for (var i = 0; i < numSamples; i++)
        {
            samples[i] = (r.ReadTimeInt32(), new byte[sampleSize]);
        }

        // Read data (Columnar Delta Decoding)
        // The data is stored by row (one byte for every buffer), then column (next byte for every buffer)
        // It is also delta-encoded horizontally across the buffers
        for (var i = 0; i < sampleSize; i++)
        {
            // Read a slice: one byte for every buffer
            var slice = r.ReadBytes(numSamples);

            if (slice.Length != numSamples)
            {
                throw new EndOfStreamException("Unexpected end of reading data slices.");
            }

            byte accumulator = 0; // Resets for every new row (byte index)

            for (var b = 0; b < numSamples; b++)
            {
                // Apply delta: CurrentBufferVal = PreviousBufferVal + ReadVal
                accumulator += slice[b];

                samples[b].data[i] = accumulator;
            }
        }

        return samples;
    }

    private static IEnumerable<EntRecordDelta2> ReadEntRecordDeltas2(GbxReader r)
    {
        while (r.ReadBoolean(asByte: true))
        {
            yield return new()
            {
                Type = r.ReadInt32(),
                Time = r.ReadTimeInt32(),
                Data = r.ReadData()
            };
        }
    }

    private static IEnumerable<EntRecordDelta> ReadEntRecordDeltas(GbxReader r, EntRecordDesc desc)
    {
        // Reads byte on every loop until the byte is 0, should be 1 otherwise
        while (r.ReadBoolean(asByte: true))
        {
            var time = r.ReadTimeInt32();
            var data = r.ReadData(); // MwBuffer
            yield return CreateEntRecordDelta(time, data, desc);
        }
    }

    private static EntRecordDelta CreateEntRecordDelta(TimeInt32 time, byte[] data, EntRecordDesc desc)
    {
        EntRecordDelta? delta = desc.ClassId switch
        {
            0x0A018000 => new CSceneVehicleVis.EntRecordDelta(time, data),
            _ => null
        };

        if (delta is null)
        {
            return new(time, data);
        }

        if (data.Length > 0)
        {
            using var ms = new MemoryStream(data);
            using var rr = new GbxReader(ms);

            delta.Read(ms, rr);

            var sampleProgress = (int)ms.Position;
        }

        return delta;
    }

    private void WriteEntList(GbxWriter w, int version)
    {
        w.Write(entList.Count > 0, asByte: true);

        for (var i = 0; i < entList.Count; i++)
        {
            var elem = entList[i];

            w.Write(elem.Type);
            w.Write(elem.U01);
            w.Write(elem.U02);
            w.Write(elem.U03);

            if (version >= 6)
            {
                w.Write(elem.U04);
            }

            var desc = entRecordDescs[elem.Type];
            if (version < 11)
            {
                WriteEntRecordDeltas(w, elem.Samples);
            }
            else
            {
                WriteEncodedDeltas(w, elem.Samples);
            }

            var hasNext = i < entList.Count - 1;
            w.Write(hasNext, asByte: true);

            if (version >= 2)
            {
                WriteEntRecordDeltas2(w, elem.Samples2);
            }
        }
    }

    private static void WriteEncodedDeltas(GbxWriter w, List<EntRecordDelta> samples)
    {
        w.Write(samples.Count);

        if (samples.Count == 0)
        {
            return;
        }

        var sampleSize = samples.Count > 0 ? samples[0].Data.Length : 0;
        w.Write(sampleSize);

        var prevTime = TimeInt32.Zero;
        foreach (var sample in samples)
        {
            w.Write(sample.Time - prevTime);
            prevTime = sample.Time;
        }

        for (var i = 0; i < sampleSize; i++)
        {
            byte accumulator = 0;
            for (var b = 0; b < samples.Count; b++)
            {
                var value = samples[b].Data[i];
                var delta = (byte)(value - accumulator);
                w.Write(delta);
                accumulator = value;
            }
        }
    }

    private static void WriteEntRecordDeltas2(GbxWriter w, List<EntRecordDelta2> samples)
    {
        foreach (var sample in samples)
        {
            w.Write(true, asByte: true);
            w.Write(sample.Type);
            w.Write(sample.Time);
            w.WriteData(sample.Data);
        }
        w.Write(false, asByte: true);
    }

    private static void WriteEntRecordDeltas(GbxWriter w, List<EntRecordDelta> samples)
    {
        foreach (var sample in samples)
        {
            w.Write(true, asByte: true);
            w.Write(sample.Time);
            w.WriteData(sample.Data);
        }
        w.Write(false, asByte: true);
    }

    private static IEnumerable<NoticeRecordListElem> ReadBulkNoticeList(GbxReader r)
    {
        while (r.ReadBoolean(asByte: true))
        {
            yield return new()
            {
                U01 = r.ReadInt32(),
                U02 = r.ReadInt32(),
                Data = r.ReadData()
            };
        }
    }

    private void WriteBulkNoticeList(GbxWriter w)
    {
        foreach (var elem in bulkNoticeList)
        {
            w.Write(true, asByte: true);
            w.Write(elem.U01);
            w.Write(elem.U02);
            w.WriteData(elem.Data);
        }
        w.Write(false, asByte: true);
    }

    private static IEnumerable<CustomModulesDeltaList> ReadCustomModulesDeltaLists(GbxReader r, int version)
    {
        var deltaListCount = version >= 8 ? r.ReadInt32() : 1;

        if (deltaListCount == 0)
        {
            yield break;
        }

        if (version >= 7)
        {
            for (var i = 0; i < deltaListCount; i++)
            {
                yield return ReadCustomModulesDeltaList(r, version);
            }
        }
    }

    private static CustomModulesDeltaList ReadCustomModulesDeltaList(GbxReader r, int version)
    {
        var deltas = new List<CustomModulesDelta>();

        while (r.ReadBoolean(asByte: true))
        {
            var u01 = r.ReadInt32();
            var data = r.ReadData(); // MwBuffer
            var u02 = version >= 9 ? r.ReadData() : [];

            deltas.Add(new()
            {
                U01 = u01,
                Data = data,
                U02 = u02
            });
        }

        var period = version >= 10 ? r.ReadInt32() : default(int?);

        return new CustomModulesDeltaList
        {
            Deltas = deltas,
            Period = period
        };
    }

    private void WriteCustomModulesDeltaLists(GbxWriter w, int version)
    {
        var deltaListCount = version >= 8 ? customModulesDeltaLists.Count : (customModulesDeltaLists.Count > 0 ? 1 : 0);

        if (version >= 8)
        {
            w.Write(deltaListCount);
        }

        if (deltaListCount == 0)
        {
            return;
        }

        if (version >= 7)
        {
            var count = version >= 8 ? deltaListCount : 1;
            for (var i = 0; i < count; i++)
            {
                WriteCustomModulesDeltaList(w, customModulesDeltaLists[i], version);
            }
        }
    }

    private static void WriteCustomModulesDeltaList(GbxWriter w, CustomModulesDeltaList deltaList, int version)
    {
        foreach (var delta in deltaList.Deltas)
        {
            w.Write(true, asByte: true);
            w.Write(delta.U01);
            w.WriteData(delta.Data);

            if (version >= 9)
            {
                w.WriteData(delta.U02);
            }
        }
        w.Write(false, asByte: true);

        if (version >= 10)
        {
            w.Write(deltaList.Period ?? 0);
        }
    }

    public partial class EntRecordDesc
    {
        public override string ToString()
        {
            var name = ClassManager.GetName(classId);
            return string.IsNullOrEmpty(name)
                ? $"0x{classId:X8}"
                : $"{name} (0x{classId:X8})";
        }
    }

    public partial class NoticeRecordDesc
    {
        public override string ToString()
        {
            if (classId is null)
            {
                return $"{U01}, {U02}";
            }

            var name = ClassManager.GetName(classId.Value);
            return string.IsNullOrEmpty(name)
                ? $"0x{classId:X8} {U01}, {U02}"
                : $"{name} (0x{classId:X8}) {U01}, {U02}";
        }
    }

    public sealed class EntRecordListElem
    {
        public int Type { get; set; }
        public int U01 { get; set; }
        public int U02 { get; set; }
        public int U03 { get; set; }
        public int U04 { get; set; }
        public List<EntRecordDelta> Samples { get; set; } = [];
        public List<EntRecordDelta2> Samples2 { get; set; } = [];
    }

    public class EntRecordDelta
    {
        public TimeInt32 Time { get; }
        public byte[] Data { get; }

        public EntRecordDelta(TimeInt32 time, byte[] data)
        {
            Time = time;
            Data = data;
        }

        public virtual void Read(MemoryStream ms, GbxReader r)
        {
            
        }

        public override string ToString()
        {
            return $"{Time}, {Data.Length} bytes";
        }
    }

    public class EntRecordDelta2
    {
        public TimeInt32 Time { get; set; }
        public int Type { get; set; }
        public byte[] Data { get; set; } = [];

        public override string ToString()
        {
            return $"{Time}, type {Type}, {Data.Length} bytes";
        }
    }

    public class NoticeRecordListElem
    {
        public int U01 { get; set; }
        public int U02 { get; set; }
        public byte[] Data { get; set; } = [];
    }

    public class CustomModulesDeltaList
    {
        public List<CustomModulesDelta> Deltas { get; set; } = [];
        public int? Period { get; set; }
    }

    public class CustomModulesDelta
    {
        public int U01 { get; set; }
        public byte[] Data { get; set; } = [];
        public byte[] U02 { get; set; } = [];
    }
}
