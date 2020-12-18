using System;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;


public class PathMovement : SystemBase
{
	const float k_walkSpeed = 3f;


    protected override void OnUpdate()
    {
        var settings = GetSingleton<CommonSettings>();
        var data = GetSingletonEntity<CommonData>();
        
        var tileBuffer = GetBufferFromEntity<TileState>()[data];
		var deltaTime = Time.DeltaTime;

		Entities
			.WithAll<Farmer>()
            .ForEach((Entity entity, ref DynamicBuffer<PathNode> pathNodes, ref Translation translation) =>
            {
                var farmerPosition = new int2((int)math.floor(translation.Value.x), 
                                           (int)math.floor(translation.Value.z));

                if (pathNodes.Length > 0)
                {
                    for (int i = 0; i < pathNodes.Length - 1; i++)
                    {
                        Debug.DrawLine(new Vector3(pathNodes[i].Value.x + .5f, .5f, pathNodes[i].Value.y + .5f), new Vector3(pathNodes[i + 1].Value.x + .5f, .5f, pathNodes[i + 1].Value.y + .5f), Color.red);
                    }

                    var nextTile = pathNodes[pathNodes.Length - 1].Value;

                    if (farmerPosition.x == nextTile.x && farmerPosition.y == nextTile.y)
                    {
                        pathNodes.RemoveAt(pathNodes.Length - 1);
                    }
                    else
                    {
                        bool isBlocked = false;
                        if (nextTile.x < 0 || nextTile.y < 0 || nextTile.x >= settings.GridSize.x || nextTile.y >= settings.GridSize.y)
                        {
                            isBlocked |= true;
                        }
                        var nextTileState = tileBuffer[nextTile.x + nextTile.y * settings.GridSize.x].Value;
                        if (nextTileState == ETileState.Rock)
                        {
                            isBlocked |= true;
                        }

                        if (!isBlocked)
                        {
                            float offset = .5f;
                            if (nextTileState == ETileState.Grown)
                            {
                                offset = .01f;
							}
							float3 targetPos = new float3(nextTile.x + offset, 0.0f, nextTile.y + offset);
							translation.Value = DroneMovement.MoveTowards(translation.Value, targetPos, k_walkSpeed * deltaTime);
                        }
                    }
                }
            }).Run();
    }
}