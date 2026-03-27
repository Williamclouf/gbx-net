using GBX.NET.Components;
using GBX.NET.Engines.Game;
using GBX.NET.Serialization;
using GBX.NET.Serialization.Chunking;

namespace GBX.NET.Tests.Unit.Serialization;

public class GbxHeaderReaderTests
{
    [Fact]
    public void ReadUserData_EmptyUserData_ReturnsFalse()
    {
        // Arrange
        using var ms = new MemoryStream(BitConverter.GetBytes(0).ToArray());
        using var r = new GbxReader(ms);

        var parser = new GbxHeaderReader(r);

        // Act
        var result = parser.ReadUserData(r, node: null, unknownHeader: null);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void ReadUserData_UserDataWithZeroHeaderChunks_ReturnsFalse()
    {
        // Arrange
        using var ms = new MemoryStream(
                    BitConverter.GetBytes(4)
            .Concat(BitConverter.GetBytes(0))
                .ToArray());
        using var r = new GbxReader(ms);

        var parser = new GbxHeaderReader(r);

        // Act
        var result = parser.ReadUserData(r, node: null, unknownHeader: null);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void ReadUserData_UnknownNodeAndNoHeaderObject_Throws()
    {
        // Arrange
        using var ms = new MemoryStream(
                    BitConverter.GetBytes(16)
            .Concat(BitConverter.GetBytes(1))
            .Concat(BitConverter.GetBytes(0x03043069))
            .Concat(BitConverter.GetBytes(4))
            .Concat(BitConverter.GetBytes(6))
                .ToArray());
        using var r = new GbxReader(ms);

        var parser = new GbxHeaderReader(r);

        // Act & Assert
        Assert.Throws<Exception>(() => parser.ReadUserData(r, node: null, unknownHeader: null));
    }

    [Fact]
    public void ReadUserData_UnknownNode_ReadsAndAddsUnknownHeaderChunk()
    {
        // Arrange
        using var ms = new MemoryStream(
                    BitConverter.GetBytes(16)
            .Concat(BitConverter.GetBytes(1))
            .Concat(BitConverter.GetBytes(0x03043069))
            .Concat(BitConverter.GetBytes(4))
            .Concat(BitConverter.GetBytes(6))
                .ToArray());
        using var r = new GbxReader(ms);

        var parser = new GbxHeaderReader(r);

        var unknownHeader = new GbxHeaderUnknown(GbxHeaderBasic.Default, 0x03043000);

        // Act
        var result = parser.ReadUserData(r, node: null, unknownHeader);

        // Assert
        Assert.True(result);
        Assert.Single(unknownHeader.UserData);
        Assert.False(unknownHeader.UserData.First().IsHeavy, "Header chunk is heavy but should not be.");
        Assert.Equal(expected: (uint)0x03043069, actual: unknownHeader.UserData.First().Id);
        Assert.Equal(expected: [6, 0, 0, 0], actual: ((HeaderChunk)unknownHeader.UserData.First()).Data);
        Assert.Equal(expected: 20, actual: ms.Position);
    }

    [Fact]
    public void ReadUserData_KnownNode_ReadsAndAddsUnknownHeaderChunk()
    {
        // Arrange
        using var ms = new MemoryStream(
                    BitConverter.GetBytes(16)
            .Concat(BitConverter.GetBytes(1))
            .Concat(BitConverter.GetBytes(0x03043069))
            .Concat(BitConverter.GetBytes(4))
            .Concat(BitConverter.GetBytes(6))
                .ToArray());
        using var r = new GbxReader(ms);

        var parser = new GbxHeaderReader(r);

        var node = new CGameCtnChallenge();

        // Act
        var result = parser.ReadUserData(r, node, unknownHeader: null);

        // Assert
        Assert.True(result);
        Assert.Single(node.Chunks);
        Assert.IsType<HeaderChunk>(node.Chunks.First());
        Assert.False(((HeaderChunk)node.Chunks.First()).IsHeavy, "Header chunk is heavy but should not be.");
        Assert.Equal(expected: (uint)0x03043069, actual: node.Chunks.First().Id);
        Assert.Equal(expected: [6, 0, 0, 0], actual: ((HeaderChunk)node.Chunks.First()).Data);
        Assert.Equal(expected: 20, actual: ms.Position);
    }

    [Fact]
    public void ReadUserData_KnownNode_CreatesAndReadsKnownHeaderChunk()
    {
        // Arrange
        using var ms = new MemoryStream(
                    BitConverter.GetBytes(16)
            .Concat(BitConverter.GetBytes(1))
            .Concat(BitConverter.GetBytes(0x03043004))
            .Concat(BitConverter.GetBytes(4))
            .Concat(BitConverter.GetBytes(6))
                .ToArray());
        using var r = new GbxReader(ms);

        var parser = new GbxHeaderReader(r);

        var node = new CGameCtnChallenge();

        // Act
        var result = parser.ReadUserData(r, node, unknownHeader: null);

        // Assert
        Assert.True(result);
        Assert.Single(node.Chunks);
        Assert.IsType<CGameCtnChallenge.HeaderChunk03043004>(node.Chunks.First());
        Assert.False(((CGameCtnChallenge.HeaderChunk03043004)node.Chunks.First()).IsHeavy, "Header chunk is heavy but should not be.");
        Assert.Equal(expected: 6, actual: ((CGameCtnChallenge.HeaderChunk03043004)node.Chunks.First()).Version);
        Assert.Equal(expected: 20, actual: ms.Position);
    }

    [Fact]
    public void Parse_KnownNode_Version6_ParsesCorrectly()
    {
        // Arrange
        using var ms = new MemoryStream(new byte[] {
            (byte)'G',
            (byte)'B',
            (byte)'X',
            6, 0,
            (byte)'B',
            (byte)'U',
            (byte)'C',
            (byte)'R' }
            .Concat(BitConverter.GetBytes(0x03043000))
            .Concat(BitConverter.GetBytes(16))
            .Concat(BitConverter.GetBytes(1))
            .Concat(BitConverter.GetBytes(0x03043004))
            .Concat(BitConverter.GetBytes(4))
            .Concat(BitConverter.GetBytes(6))
            .Concat(BitConverter.GetBytes(69))
                .ToArray());
        using var r = new GbxReader(ms);

        var parser = new GbxHeaderReader(r);

        // Act
        var header = parser.Parse(out var node);

        // Assert
        Assert.Equal(expected: 6, actual: header.Basic.Version);
        Assert.Equal(expected: GbxFormat.Binary, actual: header.Basic.Format);
        Assert.Equal(expected: GbxCompression.Uncompressed, actual: header.Basic.CompressionOfRefTable);
        Assert.Equal(expected: GbxCompression.Compressed, actual: header.Basic.CompressionOfBody);
        Assert.Equal(expected: GbxUnknownByte.R, actual: header.Basic.UnknownByte);
        Assert.Equal(expected: (uint)0x03043000, actual: header.ClassId);
        Assert.IsType<GbxHeader<CGameCtnChallenge>>(header);
        Assert.IsType<CGameCtnChallenge>(node);
        Assert.Single(node.Chunks);
        Assert.Equal(expected: 69, actual: header.NumNodes);
    }

    [Fact]
    public void Parse_KnownNode_VersionLowerThan6_ParsesCorrectly()
    {
        // Arrange
        using var ms = new MemoryStream(new byte[] {
            (byte)'G',
            (byte)'B',
            (byte)'X',
            4, 0,
            (byte)'B',
            (byte)'U',
            (byte)'C',
            (byte)'R' }
            .Concat(BitConverter.GetBytes(0x03043000))
            .Concat(BitConverter.GetBytes(69))
                .ToArray());
        using var r = new GbxReader(ms);

        var parser = new GbxHeaderReader(r);

        // Act
        var header = parser.Parse(out var node);

        // Assert
        Assert.Equal(expected: 4, actual: header.Basic.Version);
        Assert.Equal(expected: GbxFormat.Binary, actual: header.Basic.Format);
        Assert.Equal(expected: GbxCompression.Uncompressed, actual: header.Basic.CompressionOfRefTable);
        Assert.Equal(expected: GbxCompression.Compressed, actual: header.Basic.CompressionOfBody);
        Assert.Equal(expected: GbxUnknownByte.R, actual: header.Basic.UnknownByte);
        Assert.Equal(expected: (uint)0x03043000, actual: header.ClassId);
        Assert.IsType<GbxHeader<CGameCtnChallenge>>(header);
        Assert.IsType<CGameCtnChallenge>(node);
        Assert.Empty(node.Chunks);
        Assert.Equal(expected: 69, actual: header.NumNodes);
    }

    [Fact]
    public void Parse_UnknownNode_Version6_ParsesCorrectly()
    {
        // Arrange
        using var ms = new MemoryStream(new byte[] {
            (byte)'G',
            (byte)'B',
            (byte)'X',
            6, 0,
            (byte)'B',
            (byte)'U',
            (byte)'C',
            (byte)'R' }
            .Concat(BitConverter.GetBytes(0x03999000))
            .Concat(BitConverter.GetBytes(16))
            .Concat(BitConverter.GetBytes(1))
            .Concat(BitConverter.GetBytes(0x03999004))
            .Concat(BitConverter.GetBytes(4))
            .Concat(BitConverter.GetBytes(6))
            .Concat(BitConverter.GetBytes(69))
                .ToArray());
        using var r = new GbxReader(ms);

        var parser = new GbxHeaderReader(r);

        // Act
        var header = parser.Parse(out var node);

        // Assert
        Assert.Equal(expected: 6, actual: header.Basic.Version);
        Assert.Equal(expected: GbxFormat.Binary, actual: header.Basic.Format);
        Assert.Equal(expected: GbxCompression.Uncompressed, actual: header.Basic.CompressionOfRefTable);
        Assert.Equal(expected: GbxCompression.Compressed, actual: header.Basic.CompressionOfBody);
        Assert.Equal(expected: GbxUnknownByte.R, actual: header.Basic.UnknownByte);
        Assert.Equal(expected: (uint)0x03999000, actual: header.ClassId);
        Assert.IsType<GbxHeaderUnknown>(header);
        Assert.Single(((GbxHeaderUnknown)header).UserData);
        Assert.Null(node);
        Assert.Equal(expected: 69, actual: header.NumNodes);
    }

    [Fact]
    public void Parse_UnknownNode_VersionLowerThan6_ParsesCorrectly()
    {
        // Arrange
        using var ms = new MemoryStream(new byte[] {
            (byte)'G',
            (byte)'B',
            (byte)'X',
            4, 0,
            (byte)'B',
            (byte)'U',
            (byte)'C',
            (byte)'R' }
            .Concat(BitConverter.GetBytes(0x03999000))
            .Concat(BitConverter.GetBytes(69))
                .ToArray());
        using var r = new GbxReader(ms);

        var parser = new GbxHeaderReader(r);

        // Act
        var header = parser.Parse(out var node);

        // Assert
        Assert.Equal(expected: 4, actual: header.Basic.Version);
        Assert.Equal(expected: GbxFormat.Binary, actual: header.Basic.Format);
        Assert.Equal(expected: GbxCompression.Uncompressed, actual: header.Basic.CompressionOfRefTable);
        Assert.Equal(expected: GbxCompression.Compressed, actual: header.Basic.CompressionOfBody);
        Assert.Equal(expected: GbxUnknownByte.R, actual: header.Basic.UnknownByte);
        Assert.Equal(expected: (uint)0x03999000, actual: header.ClassId);
        Assert.IsType<GbxHeaderUnknown>(header);
        Assert.Empty(((GbxHeaderUnknown)header).UserData);
        Assert.Null(node);
        Assert.Equal(expected: 69, actual: header.NumNodes);
    }
}
