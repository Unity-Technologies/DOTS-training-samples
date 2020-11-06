using System.Linq;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

public class MovementSystem : SystemBase
{
    private EntityQuery _mapCellQuery;
    protected override void OnCreate()
    {
        base.OnCreate();
        _mapCellQuery = GetEntityQuery(ComponentType.ReadOnly<MazeTag>());
    }

    protected override void OnUpdate()
    {
        var deltaTime = Time.DeltaTime;

        var mazeSize = GetSingleton<MazeSize>();

        var entities = _mapCellQuery.ToEntityArray(Allocator.TempJob);

        if (entities.Length > 0)
        {
            var mapCellEntity = entities[0];

            var buffer = GetBuffer<MapCell>(mapCellEntity).AsNativeArray();

            Entities.WithReadOnly(buffer)
                .ForEach((ref Position position, ref Direction direction, in Speed speed) =>
                {
                    if (direction.MoveState == MoveState.IDLE)
                    {
                        
                        var newTile = (int2) (math.round(position.Value) + mazeSize.Value / 2 + direction.Value);

                        var mapCell = MapUtil.GetTile(in buffer, newTile, mazeSize.Value.x);
                        
                        if (direction.Value.x > 0)
                        {
                            if ((mapCell.Value & (byte) WallBits.Left) != 0)
                            {
                                direction.Value.x = 0f;
                            }
                        }
                        else if (direction.Value.x < 0)
                        {
                            if ((mapCell.Value & (byte) WallBits.Right) != 0)
                            {
                                direction.Value.x = 0f;
                            }
                        }

                        if (direction.Value.y > 0)
                        {
                            if ((mapCell.Value & (byte) WallBits.Bottom) != 0)
                            {
                                direction.Value.y = 0f;
                            }
                        }
                        else if (direction.Value.y < 0)
                        {
                            if ((mapCell.Value & (byte) WallBits.Top) != 0)
                            {
                                direction.Value.y = 0f;
                            }
                        }

                        direction.TargetTile = (int2) (math.floor(position.Value) + mazeSize.Value / 2 + direction.Value);
                        direction.MoveState = MoveState.MOVING;
                    }
                    else if (direction.MoveState == MoveState.MOVING)
                    {
                        position.Value += direction.Value * speed.Value * deltaTime;
                        if ( math.all
                            (direction.TargetTile ==  ((int2) math.floor(position.Value) + mazeSize.Value / 2)))
                        {
                            direction.MoveState = MoveState.IDLE;
                        }
                    }

                    position.Value = math.clamp(position.Value, -mazeSize.Value / 2 + 1, mazeSize.Value / 2 - 1);
                }).ScheduleParallel();
        }
    }
}
