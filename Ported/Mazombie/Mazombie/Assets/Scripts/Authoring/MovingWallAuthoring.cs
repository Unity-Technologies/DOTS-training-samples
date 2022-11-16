using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public class MovingWallAuthoring : MonoBehaviour
{
    public float wallMoveSpeed;
   
}

public class MovingWallBaker : Baker<MovingWallAuthoring>
{
    public override void Bake(MovingWallAuthoring authoring)
    {
        AddComponent(new MovingWall
        {
        });

        AddComponent(new Speed
        {
            speed = authoring.wallMoveSpeed
        });

        AddComponent(new GridPosition
        {
            gridX = 0,
            gridY = 0
        });
    }
}