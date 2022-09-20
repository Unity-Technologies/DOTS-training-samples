using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

readonly partial struct MovingWallAspect : IAspect
{
    public readonly Entity Self;

    readonly TransformAspect Transform;

    readonly RefRW<MovingWall> MovingWall;

    public float3 Position
    {
        get => Transform.Position;
        set => Transform.Position = value;
    }

    public float MoveTimer
    {
        get => MovingWall.ValueRO.MoveTimer;
        set => MovingWall.ValueRW.MoveTimer = value;
    }

    public float MoveSpeedInSeconds
    {
        get => MovingWall.ValueRO.MoveSpeedInSeconds;
    }

    public bool MovingLeft
    {
        get => MovingWall.ValueRO.MovingLeft;
        set => MovingWall.ValueRW.MovingLeft = value;
    }

    public int StartXIndex
    {
        get => MovingWall.ValueRO.StartXIndex;
    }

    public int CurrentXIndex
    {
        get => MovingWall.ValueRO.CurrentXIndex;
        set => MovingWall.ValueRW.CurrentXIndex = value;
    }

    public int NumberOfTilesToMove
    {
        get => MovingWall.ValueRO.NumberOfTilesToMove;
        set => MovingWall.ValueRW.NumberOfTilesToMove = value;
    }
}
