using Unity.Collections;
using Unity.Mathematics;

public static class Utility
{
    
    public static float2 GetNearestPos(in float2 pos, in NativeArray<Pos> positions, out int nearestIndex)
    {
        nearestIndex = 0;
        float nearestDist = float.MaxValue;
        float2 nearestPos = pos;

        for (int i = 0; i < positions.Length; i++)
        {
            float dist = math.distance(pos, positions[i].Value);
            if (dist < nearestDist)
            {
                nearestIndex = i;
                nearestDist = dist;
                nearestPos = positions[i].Value;
            }
        }

        return nearestPos;
    }
}