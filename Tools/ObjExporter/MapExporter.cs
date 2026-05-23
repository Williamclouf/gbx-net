using GBX.NET;
using GBX.NET.Engines.Game;
using GBX.NET.Engines.GameData;
using GBX.NET.Engines.Plug;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Numerics;

/// <summary>
/// Exporter for CGameCtnChallenge maps to OBJ format.
/// Merges all block geometries into a single OBJ file with proper coordinate transformations.
/// </summary>
public static class MapExporter
{
    private const string MtlLibName = "map.mtl";

    /// <summary>
    /// Exports a CGameCtnChallenge (map) to OBJ format.
    /// </summary>
    /// <param name="challenge">The map to export.</param>
    /// <param name="objFileName">Output OBJ file path.</param>
    /// <param name="mergeVerticesDigitThreshold">Digit precision for merging vertices (default: 3).</param>
    public static void ExportToObj(this CGameCtnChallenge challenge, string objFileName, int mergeVerticesDigitThreshold = 3)
    {
        var hasBlocks = challenge.Blocks?.Count > 0;
        var hasAnchoredObjects = challenge.AnchoredObjects?.Count > 0;

        if (!hasBlocks && !hasAnchoredObjects)
        {
            Console.WriteLine("Warning: Map contains no blocks or items.");
            return;
        }

        Console.WriteLine($"Exporting map '{challenge.MapName}'...");

        if (hasBlocks)
            Console.WriteLine($"  Built-in blocks: {challenge.Blocks!.Count}");
        if (hasAnchoredObjects)
            Console.WriteLine($"  Custom items: {challenge.AnchoredObjects!.Count}");

        var mtlFileName = Path.ChangeExtension(objFileName, ".mtl");
        var meshData = new MergedMeshData();

        // Process built-in blocks
        if (hasBlocks)
        {
            foreach (var block in challenge.Blocks!)
            {
                try
                {
                    var blockModelName = block.BlockModel.Id ?? block.Name;
                    if (string.IsNullOrEmpty(blockModelName))
                    {
                        Console.WriteLine("Warning: Block has no model name, skipping...");
                        continue;
                    }

                    if (!TryLoadBlockGeometry(blockModelName, block, challenge, meshData, mergeVerticesDigitThreshold))
                    {
                        Console.WriteLine($"Warning: Could not load geometry for block model '{blockModelName}'");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error processing block at {block.Coord}: {ex.Message}");
                }
            }
        }

        // Process custom items (AnchoredObjects)
        if (hasAnchoredObjects)
        {
            foreach (var obj in challenge.AnchoredObjects!)
            {
                try
                {
                    var itemName = obj.ItemModel?.Id;
                    if (string.IsNullOrEmpty(itemName))
                    {
                        Console.WriteLine("Warning: AnchoredObject has no item model, skipping...");
                        continue;
                    }

                    if (!TryLoadAnchoredGeometry(itemName, obj, challenge, meshData, mergeVerticesDigitThreshold))
                    {
                        Console.WriteLine($"Warning: Could not load geometry for item '{itemName}'");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error processing anchored object: {ex.Message}");
                }
            }
        }

        if (meshData.Vertices.Count == 0)
        {
            Console.WriteLine("Error: No geometry data was extracted from the map.");
            return;
        }

        // Write OBJ and MTL files
        WriteObjFile(objFileName, meshData);
        WriteMtlFile(mtlFileName);

        Console.WriteLine($"Map exported successfully to {objFileName}");
        Console.WriteLine($"  Vertices: {meshData.Vertices.Count}");
        Console.WriteLine($"  Faces: {meshData.Faces.Count}");
        Console.WriteLine($"  Materials: {meshData.Materials.Count}");
    }

    /// <summary>
    /// Attempts to load block geometry from a block model identifier.
    /// Tries external file first, then falls back to embedded ZIP data.
    /// </summary>
    private static bool TryLoadBlockGeometry(
        string blockModelName,
        CGameCtnBlock block,
        CGameCtnChallenge challenge,
        MergedMeshData meshData,
        int mergeVerticesDigitThreshold)
    {
        var (itemModel, itemFilePath) = LoadItemModel(blockModelName, challenge);

        if (itemModel is null)
        {
            return false;
        }

        var visModel = itemModel.GetVisModelCustom();

        if (visModel is CPlugSolid solid)
        {
            return ExtractSolidGeometry(solid, block, meshData, blockModelName, mergeVerticesDigitThreshold);
        }

        if (visModel is CGameObjectVisModel visModelObj)
        {
            if (visModelObj.Solid is not null)
            {
                return ExtractSolidGeometry(visModelObj.Solid, block, meshData, blockModelName, mergeVerticesDigitThreshold);
            }

            if (EnsureMeshShaded(visModelObj, itemFilePath, challenge) && visModelObj.MeshShaded is not null)
            {
                return ExtractSolid2Geometry(visModelObj.MeshShaded, block, meshData, blockModelName, mergeVerticesDigitThreshold);
            }
        }

        Console.WriteLine($"Warning: Unsupported visual model type '{visModel?.GetType().Name}' for block '{blockModelName}'");
        return false;
    }

    /// <summary>
    /// Attempts to load anchored object geometry from an item model.
    /// </summary>
    private static bool TryLoadAnchoredGeometry(
        string itemName,
        CGameCtnAnchoredObject obj,
        CGameCtnChallenge challenge,
        MergedMeshData meshData,
        int mergeVerticesDigitThreshold)
    {
        var (itemModel, itemFilePath) = LoadItemModel(itemName, challenge);

        if (itemModel is null)
        {
            return false;
        }

        // Build world transform from anchored object position/rotation
        var absPosition = (Vector3)(Vec3)obj.AbsolutePositionInMap;
        var yawPitchRoll = obj.YawPitchRoll;
        var pivotOffset = (Vector3)(Vec3)obj.PivotPosition;
        var rotationMatrix = Matrix4x4.CreateFromYawPitchRoll(yawPitchRoll.X, yawPitchRoll.Y, yawPitchRoll.Z);
        // T = rotate(pos - pivot) + absPos  →  bake into a single matrix
        var worldTransform = Matrix4x4.CreateTranslation(-pivotOffset) * rotationMatrix * Matrix4x4.CreateTranslation(absPosition);


        if (itemModel.EntityModelEdition is CGameCommonItemEntityModelEdition { MeshCrystal: not null } edition)
        {
            return ExtractCrystalGeometry(edition.MeshCrystal, worldTransform, meshData, itemName, mergeVerticesDigitThreshold);
        }
        else if (itemModel.EntityModelEdition is CGameBlockItem block)
        {
            bool any = false;

            foreach (var variant in block.CustomizedVariants)
            {
                if (variant.Crystal is null)
                {
                    continue;
                }

                any |= ExtractCrystalGeometry(variant.Crystal, worldTransform, meshData, $"{itemName}_{variant.Id}", mergeVerticesDigitThreshold);
            }

            return any;
        }
        else if (itemModel.EntityModel is CGameCommonItemEntityModel { StaticObject.Mesh: not null } model)
        {
            return ExtractAnchoredSolid2Geometry(model.StaticObject.Mesh, obj, meshData, itemName, mergeVerticesDigitThreshold);
        }
        else if (itemModel.GetVisModelCustom() is CGameObjectVisModel visModelObj)
        {
            if (visModelObj.Solid is not null)
            {
                return ExtractAnchoredSolidGeometry(visModelObj.Solid, obj, meshData, itemName, mergeVerticesDigitThreshold);
            }

            if (EnsureMeshShaded(visModelObj, itemFilePath, challenge) && visModelObj.MeshShaded is not null)
            {
                return ExtractAnchoredSolid2Geometry(visModelObj.MeshShaded, obj, meshData, itemName, mergeVerticesDigitThreshold);
            }
        }

        Console.WriteLine($"Warning: Item '{itemName}' has no supported mesh.");
        return false;
    }

    /// <summary>
    /// Loads a CGameItemModel from an external file or the map's embedded ZIP data.
    /// </summary>
    private static (CGameItemModel? ItemModel, string? ItemFilePath) LoadItemModel(string modelName, CGameCtnChallenge challenge)
    {
        var blocksRoot = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "TrackMania", "Blocks");
        var candidates = new[]
        {
            modelName,
            $"{modelName}.Item.Gbx",
            $"{modelName}.Gbx"
        };

        foreach (var candidate in candidates)
        {
            var blockFilePath = Path.Combine(blocksRoot, candidate);
            if (!File.Exists(blockFilePath))
            {
                continue;
            }

            try
            {
                var gbx = Gbx.Parse<CGameItemModel>(blockFilePath);
                return (gbx.Node, blockFilePath);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading external file '{blockFilePath}': {ex.Message}");
            }
        }

        // Fall back to embedded ZIP data
        if (challenge.EmbeddedZipData is { Length: > 0 })
        {
            try
            {
                using var ms = new MemoryStream(challenge.EmbeddedZipData);
                using var zip = new ZipArchive(ms, ZipArchiveMode.Read);

                foreach (var entry in zip.Entries)
                {
                    var entryName = Path.GetFileNameWithoutExtension(entry.Name);
                    if (!string.Equals(entryName, modelName, StringComparison.OrdinalIgnoreCase))
                    {
                        continue;
                    }

                    using var entryStream = entry.Open();
                    var gbx = Gbx.Parse<CGameItemModel>(entryStream);
                    return (gbx.Node, null);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading from embedded ZIP: {ex.Message}");
            }
        }

        return (null, null);
    }

    private static bool EnsureMeshShaded(CGameObjectVisModel visModel, string? itemFilePath, CGameCtnChallenge challenge)
    {
        if (visModel.MeshShaded is not null)
        {
            return true;
        }

        if (!string.IsNullOrEmpty(visModel.Mesh))
        {
            var meshPath = visModel.Mesh;

            if (!string.IsNullOrEmpty(itemFilePath))
            {
                var candidate = Path.Combine(Path.GetDirectoryName(itemFilePath) ?? string.Empty, meshPath);
                if (File.Exists(candidate))
                {
                    try
                    {
                        var mesh = Gbx.ParseNode<CPlugSolid2Model>(candidate);
                        visModel.MeshShaded = mesh;
                        visModel.Mesh = null;
                        return true;
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error loading mesh shaded from '{candidate}': {ex.Message}");
                    }
                }
            }

            if (File.Exists(meshPath))
            {
                try
                {
                    var mesh = Gbx.ParseNode<CPlugSolid2Model>(meshPath);
                    visModel.MeshShaded = mesh;
                    visModel.Mesh = null;
                    return true;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error loading mesh shaded from '{meshPath}': {ex.Message}");
                }
            }

            if (challenge.EmbeddedZipData is { Length: > 0 })
            {
                var expectedName = Path.GetFileNameWithoutExtension(meshPath);
                try
                {
                    using var ms = new MemoryStream(challenge.EmbeddedZipData);
                    using var zip = new ZipArchive(ms, ZipArchiveMode.Read);

                    foreach (var entry in zip.Entries)
                    {
                        var entryName = Path.GetFileNameWithoutExtension(entry.Name);
                        if (!string.Equals(entryName, expectedName, StringComparison.OrdinalIgnoreCase))
                        {
                            continue;
                        }

                        using var entryStream = entry.Open();
                        var mesh = Gbx.ParseNode<CPlugSolid2Model>(entryStream);
                        visModel.MeshShaded = mesh;
                        visModel.Mesh = null;
                        return true;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error loading mesh shaded from embedded ZIP: {ex.Message}");
                }
            }
        }

        if (visModel.MeshShadedFile is not null)
        {
            return visModel.GetMeshShaded() is not null;
        }

        return false;
    }

    /// <summary>
    /// Extracts geometry from a CPlugCrystal and adds it to the merged mesh.
    /// Mirrors the approach in ObjExporter.Export(CPlugCrystal, ...).
    /// Faces may be polygons (n >= 3 verts), so we fan-triangulate them.
    /// </summary>
    /// <param name="crystal">The crystal to extract geometry from.</param>
    /// <param name="transform">World-space transform matrix (rotation + translation baked in).</param>
    /// <param name="meshData">Target merged mesh data.</param>
    /// <param name="materialName">Fallback material name (used when face has no material).</param>
    /// <param name="mergeVerticesDigitThreshold">Digit precision for vertex deduplication.</param>
    /// <returns>True if any geometry was extracted.</returns>
    private static bool ExtractCrystalGeometry(
        CPlugCrystal crystal,
        Matrix4x4 transform,
        MergedMeshData meshData,
        string materialName,
        int mergeVerticesDigitThreshold)
    {
        if (crystal.Layers is null)
        {
            return false;
        }

        var allVertices = new List<Vector3>();
        var allFaces = new List<(int, int, int)>();
        int vertexOffset = 0;

        foreach (var layer in crystal.Layers)
        {
            if (layer is not CPlugCrystal.GeometryLayer { Crystal: not null } geometryLayer)
            {
                continue;
            }

            var positions = geometryLayer.Crystal.Positions;

            // Collect all transformed positions for this layer
            var layerVertices = new List<Vector3>(positions.Length);
            foreach (var pos in positions)
            {
                // Apply world transform (rotation + translation)
                var worldPos = Vector3.Transform(new Vector3(pos.X, pos.Y, pos.Z), transform);
                layerVertices.Add(worldPos);
            }

            // Fan-triangulate each face (polygon with n >= 3 verts → n-2 triangles)
            // This matches how ObjExporter writes faces: it writes all vertices per face
            // as a polygon, but for our triangle-only mesh we fan-triangulate.
            foreach (var face in geometryLayer.Crystal.Faces)
            {
                var verts = face.Vertices;
                if (verts.Length < 3)
                {
                    continue;
                }

                // Fan triangulation: (v0, v1, v2), (v0, v2, v3), (v0, v3, v4), ...
                int i0 = verts[0].Index + vertexOffset;
                for (int i = 1; i < verts.Length - 1; i++)
                {
                    int i1 = verts[i].Index + vertexOffset;
                    int i2 = verts[i + 1].Index + vertexOffset;
                    allFaces.Add((i0, i1, i2));
                }
            }

            allVertices.AddRange(layerVertices);
            vertexOffset += layerVertices.Count;
        }

        if (allVertices.Count == 0)
        {
            return false;
        }

        AddFacesToMesh(meshData, allVertices, allFaces, materialName, mergeVerticesDigitThreshold);
        return true;
    }

    /// <summary>
    /// Extracts geometry from a CPlugSolid and adds it to the merged mesh.
    /// </summary>
    private static bool ExtractSolidGeometry(
        CPlugSolid solid,
        CGameCtnBlock block,
        MergedMeshData meshData,
        string blockModelName,
        int mergeVerticesDigitThreshold)
    {
        if (solid.Tree is not CPlugTree rootTree)
        {
            return false;
        }

        var blockPosition = (Vector3)(Vec3)(block.Coord * 32); // Convert block coords to world position (32 units per block)
        var worldMatrix = GetRotationMatrixFromDirection(block.Direction);

        var allVertices = new List<Vector3>();
        var allFaces = new List<(int, int, int)>();
        int vertexOffset = 0;

        foreach (var (tree, loc) in solid.GetAllChildrenWithLocation())
        {
            if (tree.Visual is not CPlugVisualIndexedTriangles visual)
            {
                continue;
            }

            if (visual.IndexBuffer is null)
            {
                continue;
            }

            // Extract vertices with Iso4 location transform applied
            var localVertices = new List<Vector3>();

            if (visual.VertexStreams.Count > 0)
            {
                var positions = visual.VertexStreams[0].Positions;
                if (positions is not null)
                {
                    foreach (var pos in positions)
                    {
                        var locatedPos = ApplyIso4Transform(pos, loc);
                        // Apply block world transform (rotation + position)
                        var worldPos = Vector3.Transform(locatedPos.AsVector3(), worldMatrix) + blockPosition;
                        localVertices.Add(worldPos);
                    }
                }
            }
            else
            {
                foreach (var vert in visual.Vertices)
                {
                    var locatedPos = ApplyIso4Transform(vert.Position, loc);
                    var worldPos = Vector3.Transform(locatedPos.AsVector3(), worldMatrix) + blockPosition;
                    localVertices.Add(worldPos);
                }
            }

            if (localVertices.Count == 0)
            {
                continue;
            }

            // Extract triangle faces from index buffer
            var indices = visual.IndexBuffer.Indices;
            var localFaces = new List<(int, int, int)>();

            for (int i = 0; i + 2 < indices.Length; i += 3)
            {
                localFaces.Add((
                    indices[i] + vertexOffset,
                    indices[i + 1] + vertexOffset,
                    indices[i + 2] + vertexOffset));
            }

            allVertices.AddRange(localVertices);
            allFaces.AddRange(localFaces);
            vertexOffset += localVertices.Count;
        }

        if (allVertices.Count == 0)
        {
            return false;
        }

        AddFacesToMesh(meshData, allVertices, allFaces, blockModelName, mergeVerticesDigitThreshold);
        return true;
    }

    /// <summary>
    /// Extracts geometry from a CPlugSolid for an anchored object and adds it to the merged mesh.
    /// Uses absolute position and YawPitchRoll rotation instead of grid coordinates.
    /// </summary>
    private static bool ExtractAnchoredSolidGeometry(
        CPlugSolid solid,
        CGameCtnAnchoredObject obj,
        MergedMeshData meshData,
        string itemName,
        int mergeVerticesDigitThreshold)
    {
        if (solid.Tree is not CPlugTree rootTree)
        {
            return false;
        }

        var absPosition = (Vector3)(Vec3)obj.AbsolutePositionInMap;
        var yawPitchRoll = obj.YawPitchRoll;

        // YawPitchRoll: X=Yaw, Y=Pitch, Z=Roll (radians)
        var rotationMatrix = Matrix4x4.CreateFromYawPitchRoll(yawPitchRoll.X, yawPitchRoll.Y, yawPitchRoll.Z);
        var pivotOffset = (Vector3)(Vec3)obj.PivotPosition;

        var allVertices = new List<Vector3>();
        var allFaces = new List<(int, int, int)>();
        int vertexOffset = 0;

        foreach (var (tree, loc) in solid.GetAllChildrenWithLocation())
        {
            if (tree.Visual is not CPlugVisualIndexedTriangles visual)
            {
                continue;
            }

            if (visual.IndexBuffer is null)
            {
                continue;
            }

            var localVertices = new List<Vector3>();

            if (visual.VertexStreams.Count > 0)
            {
                var positions = visual.VertexStreams[0].Positions;
                if (positions is not null)
                {
                    foreach (var pos in positions)
                    {
                        var locatedPos = ApplyIso4Transform(pos, loc);
                        // Apply pivot offset, rotation, then absolute position
                        var worldPos = Vector3.Transform(locatedPos.AsVector3() - pivotOffset, rotationMatrix) + absPosition;
                        localVertices.Add(worldPos);
                    }
                }
            }
            else
            {
                foreach (var vert in visual.Vertices)
                {
                    var locatedPos = ApplyIso4Transform(vert.Position, loc);
                    var worldPos = Vector3.Transform(locatedPos.AsVector3() - pivotOffset, rotationMatrix) + absPosition;
                    localVertices.Add(worldPos);
                }
            }

            if (localVertices.Count == 0)
            {
                continue;
            }

            var indices = visual.IndexBuffer.Indices;
            var localFaces = new List<(int, int, int)>();

            for (int i = 0; i + 2 < indices.Length; i += 3)
            {
                localFaces.Add((
                    indices[i] + vertexOffset,
                    indices[i + 1] + vertexOffset,
                    indices[i + 2] + vertexOffset));
            }

            allVertices.AddRange(localVertices);
            allFaces.AddRange(localFaces);
            vertexOffset += localVertices.Count;
        }

        if (allVertices.Count == 0)
        {
            return false;
        }

        AddFacesToMesh(meshData, allVertices, allFaces, itemName, mergeVerticesDigitThreshold);
        return true;
    }

    /// <summary>
    /// Extracts geometry from a CPlugSolid2Model and adds it to the merged mesh with block transforms.
    /// </summary>
    private static bool ExtractSolid2Geometry(
        CPlugSolid2Model solid,
        CGameCtnBlock block,
        MergedMeshData meshData,
        string blockModelName,
        int mergeVerticesDigitThreshold)
    {
        if (solid.Visuals is null || solid.ShadedGeoms is null)
        {
            return false;
        }

        var blockPosition = (Vector3)(Vec3)(block.Coord * 32);
        var worldMatrix = GetRotationMatrixFromDirection(block.Direction);

        var allVertices = new List<Vector3>();
        var allFaces = new List<(int, int, int)>();
        int vertexOffset = 0;

        foreach (var geom in solid.ShadedGeoms)
        {
            if (solid.Visuals is null || geom.VisualIndex >= solid.Visuals.Length)
            {
                continue;
            }

            if (solid.Visuals[geom.VisualIndex] is not CPlugVisualIndexedTriangles visual)
            {
                continue;
            }

            if (visual.IndexBuffer is null)
            {
                continue;
            }

            var localVertices = new List<Vector3>();

            if (visual.VertexStreams.Count > 0)
            {
                var positions = visual.VertexStreams[0].Positions;
                if (positions is not null)
                {
                    foreach (var pos in positions)
                    {
                        var worldPos = Vector3.Transform(pos.AsVector3(), worldMatrix) + blockPosition;
                        localVertices.Add(worldPos);
                    }
                }
            }
            else
            {
                foreach (var vert in visual.Vertices)
                {
                    var worldPos = Vector3.Transform(vert.Position.AsVector3(), worldMatrix) + blockPosition;
                    localVertices.Add(worldPos);
                }
            }

            if (localVertices.Count == 0)
            {
                continue;
            }

            var indices = visual.IndexBuffer.Indices;
            for (int i = 0; i + 2 < indices.Length; i += 3)
            {
                allFaces.Add((
                    indices[i] + vertexOffset,
                    indices[i + 1] + vertexOffset,
                    indices[i + 2] + vertexOffset));
            }

            allVertices.AddRange(localVertices);
            vertexOffset += localVertices.Count;
        }

        if (allVertices.Count == 0)
        {
            return false;
        }

        AddFacesToMesh(meshData, allVertices, allFaces, blockModelName, mergeVerticesDigitThreshold);
        return true;
    }

    /// <summary>
    /// Extracts geometry from a CPlugSolid2Model for an anchored object and adds it to the merged mesh.
    /// </summary>
    private static bool ExtractAnchoredSolid2Geometry(
        CPlugSolid2Model solid,
        CGameCtnAnchoredObject obj,
        MergedMeshData meshData,
        string itemName,
        int mergeVerticesDigitThreshold)
    {
        if (solid.Visuals is null || solid.ShadedGeoms is null)
        {
            return false;
        }

        var absPosition = (Vector3)(Vec3)obj.AbsolutePositionInMap;
        var yawPitchRoll = obj.YawPitchRoll;
        var rotationMatrix = Matrix4x4.CreateFromYawPitchRoll(yawPitchRoll.X, yawPitchRoll.Y, yawPitchRoll.Z);
        var pivotOffset = (Vector3)(Vec3)obj.PivotPosition;

        var allVertices = new List<Vector3>();
        var allFaces = new List<(int, int, int)>();
        int vertexOffset = 0;

        foreach (var geom in solid.ShadedGeoms)
        {
            if (solid.Visuals is null || geom.VisualIndex >= solid.Visuals.Length)
            {
                continue;
            }

            if (solid.Visuals[geom.VisualIndex] is not CPlugVisualIndexedTriangles visual)
            {
                continue;
            }

            if (visual.IndexBuffer is null)
            {
                continue;
            }

            var localVertices = new List<Vector3>();

            if (visual.VertexStreams.Count > 0)
            {
                var positions = visual.VertexStreams[0].Positions;
                if (positions is not null)
                {
                    foreach (var pos in positions)
                    {
                        var worldPos = Vector3.Transform(pos.AsVector3() - pivotOffset, rotationMatrix) + absPosition;
                        localVertices.Add(worldPos);
                    }
                }
            }
            else
            {
                foreach (var vert in visual.Vertices)
                {
                    var worldPos = Vector3.Transform(vert.Position.AsVector3() - pivotOffset, rotationMatrix) + absPosition;
                    localVertices.Add(worldPos);
                }
            }

            if (localVertices.Count == 0)
            {
                continue;
            }

            var indices = visual.IndexBuffer.Indices;
            for (int i = 0; i + 2 < indices.Length; i += 3)
            {
                allFaces.Add((
                    indices[i] + vertexOffset,
                    indices[i + 1] + vertexOffset,
                    indices[i + 2] + vertexOffset));
            }

            allVertices.AddRange(localVertices);
            vertexOffset += localVertices.Count;
        }

        if (allVertices.Count == 0)
        {
            return false;
        }

        AddFacesToMesh(meshData, allVertices, allFaces, itemName, mergeVerticesDigitThreshold);
        return true;
    }

    /// <summary>
    /// Applies an Iso4 transformation to a Vec3 position.
    /// </summary>
    private static Vec3 ApplyIso4Transform(Vec3 pos, Iso4 loc)
    {
        return new Vec3(
            pos.X * loc.XX + pos.Y * loc.XY + pos.Z * loc.XZ + loc.TX,
            pos.X * loc.YX + pos.Y * loc.YY + pos.Z * loc.YZ + loc.TY,
            pos.X * loc.ZX + pos.Y * loc.ZY + pos.Z * loc.ZZ + loc.TZ
        );
    }

    /// <summary>
    /// Gets rotation matrix from block direction (0=North, 1=East, 2=South, 3=West).
    /// </summary>
    private static Matrix4x4 GetRotationMatrixFromDirection(Direction direction)
    {
        var angle = (int)direction * MathF.PI / 2f;
        return Matrix4x4.CreateRotationY(angle);
    }

    /// <summary>
    /// Adds faces to the merged mesh with vertex deduplication.
    /// </summary>
    private static void AddFacesToMesh(
        MergedMeshData meshData,
        List<Vector3> vertices,
        List<(int, int, int)> faces,
        string materialName,
        int mergeVerticesDigitThreshold)
    {
        var vertexIndexMap = new Dictionary<string, int>();
        var blockVertexIndices = new List<int>();

        // Deduplicate and add vertices
        foreach (var vertex in vertices)
        {
            var key = RoundVector(vertex, mergeVerticesDigitThreshold);
            
            if (!vertexIndexMap.TryGetValue(key, out var index))
            {
                index = meshData.Vertices.Count;
                meshData.Vertices.Add(vertex);
                vertexIndexMap[key] = index;
            }

            blockVertexIndices.Add(index);
        }

        // Ensure material exists
        if (!meshData.Materials.Contains(materialName))
        {
            meshData.Materials.Add(materialName);
        }

        // Add faces with remapped indices
        foreach (var (v0, v1, v2) in faces)
        {
            meshData.Faces.Add(new ObjFace
            {
                V1 = blockVertexIndices[v0] + 1, // OBJ indices are 1-based
                V2 = blockVertexIndices[v1] + 1,
                V3 = blockVertexIndices[v2] + 1,
                Material = materialName
            });
        }
    }

    /// <summary>
    /// Rounds a vector for vertex deduplication.
    /// </summary>
    private static string RoundVector(Vector3 v, int digits)
    {
        var factor = MathF.Pow(10f, digits);
        var x = MathF.Round(v.X * factor) / factor;
        var y = MathF.Round(v.Y * factor) / factor;
        var z = MathF.Round(v.Z * factor) / factor;
        return $"{x},{y},{z}";
    }

    /// <summary>
    /// Writes OBJ file.
    /// </summary>
    private static void WriteObjFile(string fileName, MergedMeshData meshData)
    {
        using var writer = new StreamWriter(fileName);

        writer.WriteLine($"# Map exported from GBX.NET");
        writer.WriteLine($"# Vertices: {meshData.Vertices.Count}");
        writer.WriteLine($"# Faces: {meshData.Faces.Count}");
        writer.WriteLine();
        writer.WriteLine($"mtllib {MtlLibName}");
        writer.WriteLine();

        // Write vertices
        foreach (var vertex in meshData.Vertices)
        {
            writer.WriteLine($"v {vertex.X} {vertex.Y} {vertex.Z}");
        }

        writer.WriteLine();

        // Write faces grouped by material
        var facesByMaterial = meshData.Faces.GroupBy(f => f.Material);

        foreach (var materialGroup in facesByMaterial)
        {
            writer.WriteLine($"usemtl {materialGroup.Key}");

            foreach (var face in materialGroup)
            {
                writer.WriteLine($"f {face.V1} {face.V2} {face.V3}");
            }
        }
    }

    /// <summary>
    /// Writes MTL file.
    /// </summary>
    private static void WriteMtlFile(string fileName)
    {
        using var writer = new StreamWriter(fileName);

        writer.WriteLine("# Material library for exported map");
        writer.WriteLine();
        writer.WriteLine("newmtl DefaultMaterial");
        writer.WriteLine("Ka 0.8 0.8 0.8");
        writer.WriteLine("Kd 0.8 0.8 0.8");
        writer.WriteLine("Ks 0.0 0.0 0.0");
        writer.WriteLine("Ns 10.0");
    }

    /// <summary>
    /// Represents merged mesh data during export.
    /// </summary>
    private class MergedMeshData
    {
        public List<Vector3> Vertices { get; } = new();
        public List<ObjFace> Faces { get; } = new();
        public List<string> Materials { get; } = new();
    }

    /// <summary>
    /// Represents a single OBJ face.
    /// </summary>
    private class ObjFace
    {
        public int V1 { get; set; }
        public int V2 { get; set; }
        public int V3 { get; set; }
        public string Material { get; set; } = "DefaultMaterial";
    }
}
