using GBX.NET;
using GBX.NET.Managers;

var extensions = File.ReadLines("../../../../../Resources/Extensions.txt")
    .Select(line => line.Trim().Split(' '))
    .ToDictionary(
    parts => uint.Parse(parts[0], System.Globalization.NumberStyles.HexNumber), 
    parts => (parts[1], parts.Skip(2).ToList()));

foreach (var filePath in Directory.EnumerateFiles(args[0], "*.*", SearchOption.AllDirectories))
{
    if (!Gbx.IsGbx(filePath))
    {
        continue;
    }

    Gbx gbx;

    try
    {
        gbx = Gbx.ParseHeader(filePath);
    }
    catch (Exception ex)
    {
        Console.WriteLine(filePath);
        Console.WriteLine(ex);
        continue;
    }

    var ext = GbxPath.GetExtension(filePath).TrimStart('.');

    if (string.IsNullOrEmpty(ext) || string.Equals(ext, "gbx", StringComparison.OrdinalIgnoreCase))
    {
        continue;
    }
    
    if (extensions.TryGetValue(gbx.Header.ClassId, out var extensionInfo))
    {
        var className = extensionInfo.Item1;
        var exts = extensionInfo.Item2;

        if (!exts.Contains(ext, StringComparer.OrdinalIgnoreCase))
        {
            Console.WriteLine($"Adding extension '{ext}' (example: {Path.GetFileName(filePath)}) for class ID 0x{gbx.Header.ClassId:X8} ({className})");
            exts.Add(ext);
        }
    }
    else if (ClassManager.GetName(gbx.Header.ClassId) is string className)
    {
        Console.WriteLine($"Adding extension '{ext}' (example: {Path.GetFileName(filePath)}) for class ID 0x{gbx.Header.ClassId:X8} ({className})");
        extensions[gbx.Header.ClassId] = (className, [ext]);
    }
}

File.WriteAllLines("../../../../../Resources/Extensions.txt", extensions.Select(kvp =>
{
    var classIdStr = kvp.Key.ToString("X8", System.Globalization.CultureInfo.InvariantCulture);
    var className = kvp.Value.Item1;
    var exts = kvp.Value.Item2;
    return $"{classIdStr} {className} {string.Join(' ', exts)}";
}).Order());