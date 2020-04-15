using System;
using Unity.Collections;

public struct GridData : IDisposable
{
    public static GridData Instance;

    public int Width;
    public int Height;

    public NativeArray<float> Heat;

    public void Dispose()
    {
        if (Heat.IsCreated)
        {
            Heat.Dispose();
        }
    }
}
