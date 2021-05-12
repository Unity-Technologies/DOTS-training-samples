using System;
using Unity.Entities;
using Unity.Mathematics;

public static class TraceUtils
{
    static readonly float s_MarchDistance = (float)math.sqrt(2.0f);

    public static void TraceArc(
        in float3 sourceWorldPosition,
        in float3 destinationWorldPosition,
        in BoardSize boardSize,
        in DynamicBuffer<OffsetList> offsets,
        out float3 outDestinationPos,
        out Arc outArc) 
    {
        int2 startPosition = CoordUtils.ClampPos(CoordUtils.WorldToBoardPosition(sourceWorldPosition), boardSize.Value);
        int2 endPosition = CoordUtils.ClampPos(CoordUtils.WorldToBoardPosition(destinationWorldPosition), boardSize.Value);
        float3 hitPosition = new float3(0,0,0);
        TraceBoardFindMax(startPosition, endPosition, offsets, boardSize.Value.x, boardSize.Value.y, out hitPosition);

        float3 displacementVec = destinationWorldPosition - sourceWorldPosition;
        float2 displacementVec2d = new float2(displacementVec.x, displacementVec.z);
        float totalLen = math.length(displacementVec2d);

        float3 hitDisplacementVec = hitPosition - sourceWorldPosition;
        float2 hitDisplacementVec2s = new float2(hitDisplacementVec.x, hitDisplacementVec.z);
        float hitLen = math.length(hitDisplacementVec2s);

        float hitT = math.clamp(hitLen / totalLen, 0.1f, 0.99f);

        float a, b, c;
        ParabolaUtil.CreateParabolaOverPoint(sourceWorldPosition.y, hitT, hitPosition.y, destinationWorldPosition.y, out a, out b, out c);
        outArc.Value.x = a;
        outArc.Value.y = b;
        outArc.Value.z = c;

        outDestinationPos = CoordUtils.BoardPosToWorldPos(endPosition, destinationWorldPosition.y);
    }

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
