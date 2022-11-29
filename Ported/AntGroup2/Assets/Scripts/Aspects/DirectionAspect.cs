using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Transforms;
using Unity.Mathematics;

readonly partial struct DirectionAspect : IAspect
{
    readonly RefRW<TargetDirection> targetDirection;
    readonly RefRW<PheromoneDirection> pheromoneDirection;
    readonly RefRW<WallDirection> wallDirection;

    readonly RefRW<CurrentDirection> currentDirection;
    
    public float CurrentDirection
    {
        get => currentDirection.ValueRO.Angle;
        set => currentDirection.ValueRW.Angle = value;
    }

    public float TargetDirection
    {
        get => targetDirection.ValueRO.Angle;
        set => targetDirection.ValueRW.Angle = value;
    }

    public float PheromoneDirection
    {
        get => pheromoneDirection.ValueRO.Angle;
        set => pheromoneDirection.ValueRW.Angle = value;
    }

    public float WallDirection
    {
        get => wallDirection.ValueRO.Angle;
        set => wallDirection.ValueRW.Angle = value;
    }

    public bool WallBounce
    {
        get => wallDirection.ValueRO.Reverse;
        set => wallDirection.ValueRW.Reverse = value;
    }
}
