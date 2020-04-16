using System;
using Unity.Collections;
using Unity.Mathematics;

public struct GridData : IDisposable
{
    public static GridData Instance;

    public int Width;
    public int Height;

    public NativeArray<byte> Heat;

    public void Dispose()
    {
        if (Heat.IsCreated)
        {
            Heat.Dispose();
        }
    }
}
