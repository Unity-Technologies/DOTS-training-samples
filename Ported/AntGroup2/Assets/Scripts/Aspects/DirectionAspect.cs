using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Transforms;
using Unity.Mathematics;

readonly partial struct DirectionAspect : IAspect
{
    readonly RefRO<TargetDirection> targetDirection;
    readonly RefRO<PheromoneDirection> pheromoneDirection;
    readonly RefRO<WallDirection> wallDirection;

    readonly RefRW<CurrentDirection> currentDirection;

    public float TargetDirection => targetDirection.ValueRO.Angle;
    public float PheromoneDirection => pheromoneDirection.ValueRO.Angle;
    public float WallDirection => wallDirection.ValueRO.Angle;

    public float CurrentDirection
    {
        get => currentDirection.ValueRO.Angle;
        set => currentDirection.ValueRW.Angle = value;
    }
}
