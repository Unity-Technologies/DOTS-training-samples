using System;
using Unity.Entities;
using Unity.Mathematics;

public static class TraceUtils
{
    static readonly float s_MarchDistance = (float)math.sqrt(2.0f);

    public static int2 TraceBoardFindMax(
        in int2 startPosition,
        in int2 endPosition,
        in DynamicBuffer<OffsetList> offsets,
        int width, int height, out float3 boardMaxPointWorldPos)
    {
    
        int2 currentCoordI = startPosition;
        float2 currentCoord = CoordUtils.BoardPosToWorldOffset(currentCoordI);
    
        int2 foundCoordI = currentCoordI;
        float2 foundCoord = CoordUtils.BoardPosToWorldOffset(foundCoordI);
    
        float2 endCoord = CoordUtils.BoardPosToWorldOffset(endPosition);

        float maxHeightValue = -1;
        int iterations = 0;

        while (currentCoordI.x != endPosition.x || currentCoordI.y != endPosition.y)
        {
            if (iterations > 1000)
                break;

            float2 unormalizedDirVector = endCoord - currentCoord;
            float dirVectorLen = math.length(unormalizedDirVector);
            if (dirVectorLen < 0.001f)
                break;
    
            //sample
            //closest filter:
            currentCoordI = CoordUtils.WorldOffsetToBoardPosition(currentCoord);
            currentCoordI.x = math.clamp(currentCoordI.x, 0, width - 1);
            currentCoordI.y = math.clamp(currentCoordI.y, 0, height - 1);
            int sampleIndex = CoordUtils.ToIndex(currentCoordI, width, height);
            float currentValue = offsets[sampleIndex].Value;
            if (currentValue > maxHeightValue)
            {
                maxHeightValue = currentValue;
                foundCoordI = currentCoordI;
                foundCoord = currentCoord;
            }

            float2 D = unormalizedDirVector / dirVectorLen;
            currentCoord += D * s_MarchDistance;
            ++iterations;
        }

        
        boardMaxPointWorldPos.y = maxHeightValue;
        boardMaxPointWorldPos.x = foundCoord.x;
        boardMaxPointWorldPos.z = foundCoord.y;
        return foundCoordI;
    }
    
}
