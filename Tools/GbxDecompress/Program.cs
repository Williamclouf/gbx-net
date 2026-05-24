using GBX.NET;
using GBX.NET.LZO;

if (args.Length == 0)
{
    Console.WriteLine("Usage: GbxDecompress <filename> [<filename> ...]");
    Console.Write("Press any key to continue...");
    Console.ReadKey(true);
    Console.WriteLine();
    return;
}

Gbx.LZO = new Lzo();

foreach (var fileName in args)
{
    var output = Path.Combine(Path.GetDirectoryName(fileName), "Decompressed_" + Path.GetFileName(fileName));

    Gbx.Decompress(fileName, output);

    Console.WriteLine($"{fileName} successfully decompressed.");
}