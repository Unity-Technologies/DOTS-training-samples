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

        var mapCellEntities = _mapCellQuery.ToEntityArray(Allocator.TempJob);
        
        Entities.WithReadOnly(mapCellEntities).WithDisposeOnCompletion(mapCellEntities).ForEach((ref Position position, in Speed speed, in Direction direction) =>
        {
            if (mapCellEntities.Length > 0)
            {
                var buffer = GetBuffer<MapCell>(mapCellEntities[0]);
                var oldTile = (int2) math.round(position.Value) + mazeSize.Value / 2;
                var oldPosition = position.Value;
                
                position.Value += direction.Value * speed.Value * deltaTime;
                var newTile = (int2) math.round(position.Value) + mazeSize.Value / 2;
                
                if ((oldTile != newTile).x)
                {
                    var mapCell = MapUtil.GetTile(in buffer, newTile, mazeSize.Value.x);
                    if (direction.Value.x > 0)
                    {
                        if ((mapCell.Value & (byte) WallBits.Left) != 0)
                        {
                            position.Value = oldPosition;
                        }
                    }
                    else if (direction.Value.x < 0)
                    {
                        if ((mapCell.Value & (byte) WallBits.Right) != 0)
                        {
                            position.Value = oldPosition;
                        }
                    }
                }

                if ((oldTile != newTile).y)
                {
                    var mapCell = MapUtil.GetTile(in buffer, newTile, mazeSize.Value.x);
                    if (direction.Value.y > 0)
                    {
                        if ((mapCell.Value & (byte) WallBits.Bottom) != 0)
                        {
                            position.Value = oldPosition;
                        }
                    }
                    else if (direction.Value.y < 0)
                    {
                        if ((mapCell.Value & (byte) WallBits.Top) != 0)
                        {
                            position.Value = oldPosition;
                        }
                    }
                }
            }
            position.Value = math.clamp(position.Value, -mazeSize.Value/2, mazeSize.Value/2);
        }).Schedule();
    }
}
