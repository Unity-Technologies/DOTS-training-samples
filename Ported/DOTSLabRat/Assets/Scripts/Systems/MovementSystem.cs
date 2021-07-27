using DOTSRATS;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

public class Movement : SystemBase
{
    protected override void OnUpdate()
    {
        var time = (float)Time.ElapsedTime;

        var gameStateEntity = GetSingletonEntity<GameState>();
        var gameState = EntityManager.GetComponentData<GameState>(gameStateEntity);

        var cellStructs = GetBuffer<CellStruct>(gameStateEntity);

        Entities
            .WithAll<InPlay>()
            .WithReadOnly(cellStructs)
            .ForEach((ref Translation translation, ref Velocity velocity) =>
            {
                var newTranslation = translation.Value + time * velocity.Direction.ToFloat3() * velocity.Speed;

                // Are we crossing the center of a tile?
                var newTile = (int3)(newTranslation + 0.5f);
                var tileCenter = (float3)newTile + 0.5f;
                if (newTranslation.x >= tileCenter.x != translation.Value.x >= tileCenter.x ||
                    newTranslation.z >= tileCenter.z != translation.Value.z >= tileCenter.z)
                {
                    var cell = cellStructs[newTile.z * gameState.boardSize + newTile.x];

                    if (cell.hole)
                    {
                        velocity.Direction = Direction.Down;
                        newTranslation = new float3(tileCenter.x, -math.length(newTranslation - tileCenter), tileCenter.z);
                    }
                    // TODO: Goal collision
                    // if (cell.goal)
                    // {}
                    else if (cell.wallLayout == (Direction.North & Direction.South & Direction.East & Direction.West))
                    {
                        // This will only happen if something exists within a completely walled system. The spawners
                        // should probably identify this situation and immediately set velocity/direction to None.
                        velocity.Direction = Direction.None;
                        velocity.Speed = 0f;
                        translation.Value = new float3(tileCenter.x, translation.Value.y, tileCenter.z);
                    }
                    else if (cell.arrow != Direction.None || cell.wallLayout != Direction.None)
                    {
                        if (cell.arrow != Direction.None)
                            velocity.Direction = cell.arrow;
                        for (int i=0; i<3; ++i)
                            if ((velocity.Direction & cell.wallLayout) != Direction.None)
                                velocity.Direction = RotateClockWise(velocity.Direction);

                        // Tweak new translation according to the (possible) new direction
                        newTranslation =
                            tileCenter + math.length(newTranslation - tileCenter) * velocity.Direction.ToFloat3();
                    }
                }

                translation.Value = newTranslation;
            }).ScheduleParallel();
    }

    static Direction RotateClockWise(Direction dir)
    {
        switch (dir)
        {
            case Direction.North: return Direction.East;
            case Direction.South: return Direction.West;
            case Direction.East:  return Direction.South;
            case Direction.West:  return Direction.North;
            case Direction.Up:    return Direction.Up;
            case Direction.Down:  return Direction.Down;
            case Direction.None:
            default:              return Direction.None;
        }
    }
}
