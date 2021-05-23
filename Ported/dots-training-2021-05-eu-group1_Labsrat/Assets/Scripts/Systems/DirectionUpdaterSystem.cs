using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

public struct DirectionUpdate : IJobEntity
{
    [Read] public Lookup<ForcedDirection> forcedDirectionData;
    [ReadSingleton] public NativeArray<Entity> cells;
    [ReadSingleton] public NativeArray<Cardinals> walls;
    [ReadSingleton] public GameConfig gameConfig;
        
    [IncludeAny(typeof(Cat), typeof(Mouse))]
    public void Execute( ref Direction direction, ref Translation translation, ref Rotation rotation)
    {
        bool recenter = false;

        int index = Utils.WorldPositionToCellIndex(translation.Value, gameConfig);
        if (index < 0)
            return;

        Entity cell =  cells[index];
        Cardinals wallCollision = walls[index];
        ForcedDirection fd = forcedDirectionData[cell];

        float2 cellCenter = new float2(math.round(translation.Value.x), math.round(translation.Value.z));

        // If we're stepping on an arrow
        if (
            fd.Value != Cardinals.None                                                                                              // If there's a forced direction (arrow) ....
            && fd.Value != direction.Value                                                                                          // ... and we're not already in the given direction
            && Utils.SnapTest(new float2(translation.Value.x, translation.Value.z), cellCenter, direction.Value)
            )
        {
            direction.Value = fd.Value;
            translation.Value.x = cellCenter.x;
            translation.Value.z = cellCenter.y;
        }

        // Until we don't have a wall facing us
        while (
            wallCollision != Cardinals.All &&
            ((wallCollision & direction.Value) != 0) &&
            Utils.SnapTest(new float2(translation.Value.x, translation.Value.z), cellCenter, direction.Value)
            )
        {
            direction.Value = Direction.RotateLeft(direction.Value);

            translation.Value.x = cellCenter.x;
            translation.Value.z = cellCenter.y;
        }

        rotation.Value = math.slerp(rotation.Value, quaternion.RotateY(Direction.GetAngle(direction.Value)), 0.1f);
    }
}