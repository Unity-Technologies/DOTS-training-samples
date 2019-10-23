using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Collections;
using Unity.Entities;

public enum Method : byte
{
    Walk,
    Train
}

public struct Connection
{
    public int destinationPlatformId;
    public Method method;
}

public struct Path
{
    public int fromPlatformId;
    public int toPlatformId;
    public Connection[] connections;
}

public struct PathLookupBlob
{
    public BlobArray<Path> paths;
}

public struct PathLookup : IComponentData
{
    public BlobAssetReference<PathLookupBlob> value;
}

public static class PathLookupHelper
{
    public static BlobAssetReference<PathLookupBlob> CreatePathLookup(IEnumerable<Platform> platforms)
    {
        using (var blobBuilder = new BlobBuilder(Allocator.Temp))
        {
            ref var builderRoot = ref blobBuilder.ConstructRoot<PathLookupBlob>();
            var allPaths = Pathfinding.GetAllPaths(platforms).ToList();
            var pathsBuilder = blobBuilder.Allocate(ref builderRoot.paths, allPaths.Count);

            for (var i = 0; i < allPaths.Count; i++)
                pathsBuilder[i] = allPaths[i];

            return blobBuilder.CreateBlobAssetReference<PathLookupBlob>(Allocator.Persistent);
        }
    }
}
