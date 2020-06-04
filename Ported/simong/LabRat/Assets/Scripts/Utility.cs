using Unity.Mathematics;
using UnityEngine;

public class Utility
{
    public static int2 WorldPositionToGridCoordinates(float2 worldPos, float2 cellSize)
    {
        float2 worldPos2D = new float2(worldPos.x, worldPos.y);
        return new int2((int)math.floor(worldPos2D.x / cellSize.x), (int)math.floor(worldPos2D.y / cellSize.y));
    }

    public static float2 GridCoordinatesToWorldPos(int2 gridCoord, float2 cellSize)
    {
        return new float2(gridCoord.x * cellSize.x + cellSize.x / 2, gridCoord.y * cellSize.y + cellSize.y / 2);
    }

    public static float2 GridCoordinatesToScreenPos(Camera cam, int2 gridCoord, float2 cellSize)
    {
        float2 wp = GridCoordinatesToWorldPos(gridCoord, cellSize);
        Vector3 sp = cam.WorldToScreenPoint(new Vector3(wp.x, 0f, wp.y));
        float2 rv = new float2(sp.x / cam.pixelWidth, sp.y / cam.pixelHeight);
        return rv;
    }
    
    public static float DirectionToAngle(GridDirection dir)
    {
        switch (dir)
        {
            case GridDirection.NORTH:
                return math.PI;

            case GridDirection.EAST:
                return math.PI * 1.5f;

            case GridDirection.SOUTH:
                return 0;

            case GridDirection.WEST:
                return math.PI * 0.5f;

            default:
                throw new System.ArgumentOutOfRangeException("Invalid direction set");
        }
    }

    public static float2 ForwardVectorForDirection(GridDirection dir)
    {
        switch (dir)
        {
            case GridDirection.NORTH:
                return new float2(0f, 1f);

            case GridDirection.EAST:
                return new float2(1f, 0f);

            case GridDirection.SOUTH:
                return new float2(0f, -1f);

            case GridDirection.WEST:
                return new float2(-1f, 0f);

            default:
                throw new System.ArgumentOutOfRangeException("Invalid direction set");
        }
    }

    // taken from https://stackoverflow.com/questions/3874627/floating-point-comparison-functions-for-c-sharp
    // as MathF.Approximately doesn't have an equivalent in unity.mathematics
    public static bool NearlyEqual(float a, float b, float epsilon)
    {
        float absA = math.abs(a);
        float absB = math.abs(b);
        float diff = math.abs(a - b);

        if (a == b)
        {
            // shortcut, handles infinities
            return true;
        }
        else if (a == 0 || b == 0 || absA + absB < math.FLT_MIN_NORMAL)
        {
            // a or b is zero or both are extremely close to it
            // relative error is less meaningful here
            return diff < (epsilon * math.FLT_MIN_NORMAL);
        }
        else
        {
            // use relative error
            return diff / (absA + absB) < epsilon;
        }
    }
}
