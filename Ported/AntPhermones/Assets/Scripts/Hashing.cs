using System.Runtime.CompilerServices;
using Unity.Burst;

[BurstCompile]
public static class Hashing {

    // This code was mostly taken from the CLR implementaiton on GitHub.
    // https://github.com/dotnet/coreclr/blob/master/src/System.Private.CoreLib/shared/System/HashCode.cs

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static uint RotateLeft(uint value, int offset)
        => (value << offset) | (value >> (32 - offset));

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static uint MixFinal(uint hash)
    {
        const uint Prime2 = 2246822519U;
        const uint Prime3 = 3266489917U;
        hash ^= hash >> 15;
        hash *= Prime2;
        hash ^= hash >> 13;
        hash *= Prime3;
        hash ^= hash >> 16;
        return hash;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static uint QueueRound(uint hash, uint queuedValue)
    {
        const uint Prime3 = 3266489917U;
        const uint Prime4 = 668265263U;
        return RotateLeft(hash + queuedValue * Prime3, 17) * Prime4;
    }

    public static int Combine(uint hc1, uint hc2)
    {
        const uint Prime5 = 374761393U;

        uint hash = hc1 + Prime5;
        hash += 8;

        hash = QueueRound(hash, hc1);
        hash = QueueRound(hash, hc2);

        hash = MixFinal(hash);
        return (int)hash;
    }
}