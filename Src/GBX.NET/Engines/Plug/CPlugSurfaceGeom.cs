
namespace GBX.NET.Engines.Plug;

public partial class CPlugSurfaceGeom
{
    public MaterialId? SurfaceId { get; set; }

    public partial class Chunk0900D002
    {
        public override void Read(CPlugSurfaceGeom n, GbxReader r)
        {
            n.Surf = new Mesh();
            n.Surf.Read(r);
        }

        public override void Write(CPlugSurfaceGeom n, GbxWriter w)
        {
            if (n.Surf is null)
            {
                throw new InvalidOperationException("Cannot write default (null) surf.");
            }

            n.Surf.Write(w);
        }
    }

    public partial class Chunk0900F002
    {
        public override void Read(CPlugSurfaceGeom n, GbxReader r)
        {
            n.Surf = ReadSurf(r, version: 0);
            n.SurfaceId = (MaterialId)r.ReadInt16();
        }

        public override void Write(CPlugSurfaceGeom n, GbxWriter w)
        {
            WriteSurf(n.Surf, w, version: 0);
            w.Write((short)(n.SurfaceId ?? default));
        }
    }

    public partial class Chunk0900F004
    {
        public string? U01;
        public BoxAligned U02;

        public override void ReadWrite(CPlugSurfaceGeom n, GbxReaderWriter rw)
        {
            rw.Id(ref U01);
            rw.BoxAligned(ref U02);

            if (rw.Reader is not null)
            {
                rw.Reader.Settings.EncryptionInitializer?.Initialize(BitConverter.GetBytes(U02.X - U02.X2), 0, 4);

                n.Surf = ReadSurf(rw.Reader, version: 0);
                n.SurfaceId = (MaterialId)rw.Reader.ReadInt16();
            }
            
            if (rw.Writer is not null)
            {
                WriteSurf(n.Surf, rw.Writer, version: 0);
                rw.Writer.Write((short)(n.SurfaceId ?? default));
            }
        }
    }
}
