using Unity.Collections;
using Unity.Mathematics;

public class FABIK
{
    public static void Solve(NativeArray<float3> chain, float boneLength, float3 anchor, float3 target, float3 bendHint)
    {
        chain[chain.Length - 1] = target;
        for (int i = chain.Length - 2; i >= 0; i--)
        {
            chain[i] += bendHint;
            float3 delta = chain[i] - chain[i + 1];
            chain[i] = chain[i + 1] + math.normalize(delta) * boneLength;
        }

        chain[0] = anchor;
        for (int i = 1; i < chain.Length; i++)
        {
            float3 delta = chain[i] - chain[i - 1];
            chain[i] = chain[i - 1] + math.normalize(delta) * boneLength;
        }
    }
}
