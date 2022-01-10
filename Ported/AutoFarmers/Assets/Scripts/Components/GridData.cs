using System;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

public class GridData : IComponentData, IDisposable 
{
    public NativeArray<byte> groundTiles; // 0 not tilled, byte.max is tilled

    public void Dispose()
    {
        groundTiles.Dispose();
    }
}
