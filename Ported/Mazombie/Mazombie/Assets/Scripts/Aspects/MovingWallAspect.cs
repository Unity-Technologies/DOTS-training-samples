using Unity.Entities;
using Unity.Transforms;

public readonly partial struct MovingWallAspect : IAspect
{
    public readonly RefRO<MovingWall> MovingWall;
    public readonly RefRW<LocalToWorldTransform> Transform;
    public readonly RefRW<Speed> Speed;
    public readonly RefRO<GridPositions> GridPositions;
}

