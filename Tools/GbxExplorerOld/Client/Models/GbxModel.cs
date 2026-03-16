using GBX.NET;
using GBX.NET.Engines.MwFoundations;
using GBX.NET.Managers;
using System.Collections.Immutable;

namespace GbxExplorerOld.Client.Models;

public class GbxModel : GbxModelBase
{
    public Gbx Gbx { get; }
    public string FileNameWithoutExtension { get; set; }
    public string? OfficialExtension { get; set; }

    public ImmutableArray<TypeModel>? Inheritance { get; }

    public GbxModel(string fileName, DateTimeOffset lastModified, byte[] pureData, string sha256, Gbx gbx)
        : base(fileName, lastModified, pureData, sha256)
    {
        Gbx = gbx;
        FileNameWithoutExtension = Path.GetFileNameWithoutExtension(fileName);

        if (ClassManager.GetFileExtensions(gbx.Header.ClassId).Any())
        {
            FileNameWithoutExtension = Path.GetFileNameWithoutExtension(FileNameWithoutExtension);
            var extension = GbxPath.GetExtension(fileName).TrimStart('.');

            var matchingExtension = ClassManager.GetFileExtensions(gbx.Header.ClassId).FirstOrDefault(ext => string.Equals(ext, extension, StringComparison.OrdinalIgnoreCase));

            if (matchingExtension is not null)
            {
                OfficialExtension = matchingExtension;
            }
        }

        if (gbx.Node is not null)
        {
            Inheritance = GetInheritance(gbx.Node).Select(x => new TypeModel(x)).ToImmutableArray();
        }
    }

    private static IEnumerable<Type> GetInheritance(CMwNod node)
    {
        var type = node.GetType();

        while (type is not null && type != typeof(object))
        {
            yield return type;
            type = type.BaseType;
        }
    }
}
