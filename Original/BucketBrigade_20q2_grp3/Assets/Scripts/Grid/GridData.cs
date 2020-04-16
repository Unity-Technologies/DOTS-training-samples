using System;
using Unity.Collections;
using UnityEngine;

public struct GridData : IDisposable
{
    public static GridData Instance;

    public int Width;
    public int Height;

    public float CellSize;

    public NativeArray<byte> Heat;

    public void Dispose()
    {
        if (Heat.IsCreated)
        {
            Heat.Dispose();
        }
    }
}
