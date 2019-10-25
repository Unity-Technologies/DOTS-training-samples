using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;

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
    public Connection[] connections;
    public int fromPlatformId;
    public int toPlatformId;
}

public struct PathBlob
{
    public BlobArray<Connection> connections;
    public int fromPlatformId;
    public int toPlatformId;
}

public struct PathLookupBlob
{
    public BlobArray<PathBlob> paths;
}

public struct PathTable
{
    public BlobArray<Connection> connections;
    public BlobArray<int> startIndices;
    public BlobArray<int> endIndices; // Inclusive
    public BlobArray<int> startPlatformIDs;

    public Path GetPath(int index)
    {
        var start = startIndices[index];
        var end = endIndices[index];
        var count = end - start;

        // Ensure no invalid path
        if (count <= 0)
        {
            var next = (index + 1) % (startIndices.Length - 1);
            return GetPath(next);
        }

        var connectionsArray = new Connection[count];
        for (int i = 0; i < count; ++i)
        {
            connectionsArray[i] = connections[start + i];
        }

        return new Path
        {
            connections = connectionsArray,
            fromPlatformId = startPlatformIDs[index],
            toPlatformId = connectionsArray[count - 1].destinationPlatformId
        };
    }
}

public struct PathLookup : IComponentData
{
    public BlobAssetReference<PathTable> value;
}

public static class PathLookupHelper
{
    public static BlobAssetReference<PathTable> CreatePathLookup(IEnumerable<Platform> platforms)
    {
        using (var blobBuilder = new BlobBuilder(Allocator.Temp))
        {
            ref var builderRoot = ref blobBuilder.ConstructRoot<PathTable>();
            var allPaths = Pathfinding.GetAllPaths(platforms).ToList();

            var allConnectionsCount = allPaths.Sum(p => p.connections.Length);

            var connectionsBuilder = blobBuilder.Allocate(ref builderRoot.connections, allConnectionsCount);
            var startIndicesBuilder = blobBuilder.Allocate(ref builderRoot.startIndices, allPaths.Count);
            var endIndicesBuilder = blobBuilder.Allocate(ref builderRoot.endIndices, allPaths.Count);
            var startPlatformsBuilder = blobBuilder.Allocate(ref builderRoot.startPlatformIDs, allPaths.Count);

            var connectionIndex = 0;

            for (var i = 0; i < allPaths.Count; i++)
            {
                Connection[] connections = allPaths[i].connections;

                startIndicesBuilder[i] = connectionIndex;

                for (var i1 = 0; i1 !=  connections.Length; ++i1)
                    connectionsBuilder[connectionIndex++] = connections[i1];

                endIndicesBuilder[i] = connectionIndex - 1;

                startPlatformsBuilder[i] = allPaths[i].fromPlatformId;
            }

            return blobBuilder.CreateBlobAssetReference<PathTable>(Allocator.Persistent);
        }
    }
}
