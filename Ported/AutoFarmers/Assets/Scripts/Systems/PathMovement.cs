using System;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

public class PathMovement : SystemBase
{
    protected override void OnUpdate()
    {
        var pathBuffers = this.GetBufferFromEntity<PathNode>();

        var settingsEntity = GetSingletonEntity<Settings>();
        var tileBufferAccessor = this.GetBufferFromEntity<TileState>();
        var settings = GetComponentDataFromEntity<Settings>()[settingsEntity];

        Entities
            .ForEach((Entity entity, ref Velocity velocity, in Translation translation) =>
            {
                var pathNodes = pathBuffers[entity];
                var tileBuffer = tileBufferAccessor[settingsEntity];

                var farmerPosition = new int2((int)math.floor(translation.Value.x), 
                                           (int)math.floor(translation.Value.z));

                if (pathNodes.Length > 0)
                {
                    for (int i = 0; i < pathNodes.Length - 1; i++)
                    {
                        Debug.DrawLine(new Vector3(pathNodes[i].Value.x + .5f, .5f, pathNodes[i].Value.y + .5f), new Vector3(pathNodes[i + 1].Value.x + .5f, .5f, pathNodes[i + 1].Value.y + .5f), Color.red);
                    }

                    var targetPosition = pathNodes[pathNodes.Length - 1].Value;

                    if (farmerPosition.x == targetPosition.x && farmerPosition.y == targetPosition.y)
                    {
                        pathNodes.RemoveAt(pathNodes.Length - 1);
                    }
                    else
                    {
                        bool isBlocked = false;
                        if (targetPosition.x < 0 || targetPosition.y < 0 || targetPosition.x >= settings.GridSize.x || targetPosition.y >= settings.GridSize.y)
                        {
                            isBlocked |= true;
                        }
                        var nextTileState = tileBuffer[targetPosition.x + targetPosition.y * settings.GridSize.x].Value;
                        if (nextTileState == TileStates.Rock)
                        {
                            isBlocked |= true;
                        }

                        if (!isBlocked)
                        {
                            float offset = .5f;
                            if (nextTileState == TileStates.Grown)
                            {
                                offset = .01f;
                            }
                            var delta = (new float2(targetPosition.x + offset, targetPosition.y + offset)) - farmerPosition;
                            velocity.Value = math.normalize(new float3(delta.x, 0, delta.y)) * 3;
                        }
                    }
                }
                else 
                {
                    velocity.Value = 0;
                }
            }).Run();
    }

}