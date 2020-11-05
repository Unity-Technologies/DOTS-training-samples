using Unity.Burst;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;

using UnityEngine;

public static class Constants
{
    public readonly static int2 kInt2Left  = new int2(-1, 0);
    public readonly static int2 kInt2Right = new int2(+1, 0);
    public readonly static int2 kInt2Up    = new int2(0, +1);
    public readonly static int2 kInt2Down  = new int2(0, -1);
}
