using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public class MovingWallAuthoring : MonoBehaviour
{

    public int wallLength;
    public float wallMoveSpeed;
    public int wallMoveRangeMin;
    public int wallMoveRangeMax;
}

public class MovingWallBaker : Baker<MovingWallAuthoring>
{
    public override void Bake(MovingWallAuthoring authoring)
    {
        AddComponent(new MovingWall
        {
            wallLength = authoring.wallLength
        });

        AddComponent(new Speed
        {
            speed = authoring.wallMoveSpeed
        });
        
        AddComponent(new HorizontalMovementRange
        {
            rangeMin = authoring.wallMoveRangeMin,
            rangeMax = authoring.wallMoveRangeMax
        });
        
        AddComponent(new GridPosition
        {
            gridX = 0,
            gridY = 0
        });
    }
}