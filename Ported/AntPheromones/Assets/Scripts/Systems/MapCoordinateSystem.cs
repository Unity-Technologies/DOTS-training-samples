using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Mathematics;

public class MapCoordinateSystem : SystemBase
{
    protected override void OnUpdate()
    {
    }

    public static int PositionToIndex(float2 positionXY, Tuning tuning)
    {
        int xIndex = (int)math.floor(((positionXY.x / tuning.WorldSize) + tuning.WorldOffset.x));
        int yIndex = (int)math.ceil(((positionXY.y / tuning.WorldSize) + tuning.WorldOffset.y));
        int index = (int)math.clamp((yIndex * tuning.Resolution) + xIndex, 0, (tuning.Resolution * tuning.Resolution) - 1);

        return index;
    }
}
