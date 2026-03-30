namespace GBX.NET.Engines.Plug;

public partial class CPlugFileGen : IVersionable
{
    public int Version { get; set; }

    private int genKind;
    public int GenKind { get => genKind; set => genKind = value; }

    private int[]? u01;
    public int[]? U01 { get => u01; set => u01 = value; }

    private Vec4[]? u02;
    public Vec4[]? U02 { get => u02; set => u02 = value; }

    private float[]? u03;
    public float[]? U03 { get => u03; set => u03 = value; }

    private string? u04;
    public string? U04 { get => u04; set => u04 = value; }

    private CFuncKeysReal[]? u05;
    public CFuncKeysReal[]? U05 { get => u05; set => u05 = value; }

    public override void ReadWrite(GbxReaderWriter rw)
    {
        rw.VersionInt32(this);

        // If the MSB is set, it indicates the newer archive format
        if (Version < 0)
        {
            Version &= 0x7FFFFFFF;

            if (Version > 5)
            {
                throw new VersionNotSupportedException(Version);
            }

            rw.Int32(ref genKind);
            rw.Array<int>(ref u01);
            rw.Array<Vec4>(ref u02);
            rw.Array<float>(ref u03);

            if (Version > 2)
            {
                rw.String(ref u04);
            }

            if (Version > 3)
            {
                rw.ArrayNodeRef<CFuncKeysReal>(ref u05);
            }

            // Backward compatibility fixups for older versions
            if (Version == 0)
            {
                ApplyLegacyFixups();
            }
            else if (Version == 1 && genKind == 9 && u01.Length >= 5)
            {
                // Flips a boolean-like integer flag
                u01[4] = (u01[4] == 0) ? 1 : 0;
            }
        }
        else
        {
            // Older format logic where MSB is not set
            if (Version > 0x32)
            {
                // 0x32 = 50 (EGenCount)
                throw new VersionNotSupportedException(Version);
            }

            GenKind = Version;
            rw.Array<int>(ref u01);
            rw.Array<Vec4>(ref u02);

            Version = 0;

            ApplyLegacyFixups();
        }

        // If type is 9, there is raw image data appended that needs to be loaded
        if (GenKind == 9)
        {
            throw new NotImplementedException("GenKind 9 with raw image data is not supported yet.");
        }
    }

    private void ApplyLegacyFixups()
    {
        if (GenKind == 5 || GenKind == 6)
        {
            var index = (GenKind == 5) ? 2 : 1;

            // Translates logic: uVar3 = (*puVar6 != 0) + 3;
            u01[index] = (u01[index] != 0) ? 4 : 3;
        }
    }
}