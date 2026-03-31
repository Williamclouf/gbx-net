using GBX.NET.Generators.Extensions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;
using System.Collections.Immutable;
using System.Globalization;
using System.Text;

namespace GBX.NET.Generators;

[Generator]
public class ClassFileExtensionGenerator : IIncrementalGenerator
{
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        var contents = context.AdditionalTextsProvider
            .Where(static file =>
            {
                return file.Path.EndsWith("Extensions.txt") && Path.GetDirectoryName(file.Path).EndsWith("Resources");
            })
            .Select((additionalText, cancellationToken) =>
            {
                var text = additionalText.GetText(cancellationToken) ?? throw new Exception("Could not get text from file.");
                return text.Lines;
            });

        context.RegisterSourceOutput(contents, GenerateSource);
    }

    private static ImmutableDictionary<uint, List<string>> GetClassIdExtensionsDict(TextLineCollection lines)
    {
        var dict = ImmutableDictionary.CreateBuilder<uint, List<string>>();

        foreach (var line in lines)
        {
            var parts = line.ToString().Split(' ');

            if (parts.Length < 3)
            {
                continue;
            }

            var classIdStr = parts[0];

            if (!uint.TryParse(classIdStr, NumberStyles.HexNumber, CultureInfo.InvariantCulture, out var classId))
            {
                continue;
            }

            dict[classId] = parts.Skip(2).ToList();
        }

        return dict.ToImmutable();
    }

    private void GenerateSource(SourceProductionContext context, TextLineCollection lines)
    {
        var classIdDict = GetClassIdExtensionsDict(lines);

        var builder = new StringBuilder();

        builder.AppendLine("namespace GBX.NET.Managers;");
        builder.AppendLine();
        builder.AppendLine("public static partial class ClassManager");
        builder.AppendLine("{");
        builder.AppendLine("    public static partial IEnumerable<string> GetFileExtensions(uint classId)");
        builder.AppendLine("    {");
        builder.AppendLine("        return classId switch");
        builder.AppendLine("        {");

        foreach (var pair in classIdDict)
        {
            builder.Append($"            0x{pair.Key:X8} => [");

            for (var i = 0; i < pair.Value.Count; i++)
            {
                if (i > 0)
                {
                    builder.Append(", ");
                }

                var format = pair.Value[i];
                builder.Append('"');
                builder.Append(format);
                builder.Append('"');
            }

            builder.AppendLine("],");
        }

        builder.AppendLine("            _ => []");
        builder.AppendLine("        };");
        builder.AppendLine("    }");
        builder.AppendLine("}");

        context.AddSource("ClassManager.GetFileExtensions", builder.ToString());
    }
}