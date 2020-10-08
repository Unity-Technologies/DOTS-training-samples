using System;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

[UpdateBefore(typeof(CursorToUISystem))]
public class PlayerMovement : SystemBase
{
    EntityCommandBufferSystem m_ECBSystem;

    protected override void OnCreate()
    {
        m_ECBSystem = World.GetExistingSystem<EndSimulationEntityCommandBufferSystem>();
    }

    protected override void OnUpdate()
    {
        var camera = UnityEngine.Camera.main;
        if (camera == null)
            return;
        
        var ray = camera.ScreenPointToRay(UnityEngine.Input.mousePosition);
        new UnityEngine.Plane(UnityEngine.Vector3.up, -0.5f).Raycast(ray, out var enter);
        var hit = (float3)ray.GetPoint(enter);

        var clicked = UnityEngine.Input.GetMouseButtonDown(0);
        var boardSize = GetSingleton<GameInfo>().boardSize;

        var ecb = m_ECBSystem.CreateCommandBuffer().AsParallelWriter();

        Entities.WithAll<PlayerCursor>().ForEach((Entity e, int entityInQueryIndex, ref Position position) =>
        {
            position.Value = new float2(hit.x, hit.z);

            if (clicked)
            {
                var normalizedPosition = new float2(position.Value.x - math.round(position.Value.x), position.Value.y - math.round(position.Value.y));
                var dotLeft = math.dot(normalizedPosition, new float2(0.5f, 0.5f));
                var dotFront = math.dot(normalizedPosition, new float2(-0.5f, 0.5f));
                
                byte direction = 0;
                if (dotLeft < 0 && dotFront < 0)
                    direction = DirectionDefines.South;
                else if(dotLeft < 0 && dotFront > 0)
                    direction = DirectionDefines.West;
                else if(dotLeft > 0 && dotFront < 0)
                    direction = DirectionDefines.East;
                else
                    direction = DirectionDefines.North;
                
                ecb.AddComponent(entityInQueryIndex, e, new PlacingArrow
                {
                    TileIndex = (int)math.round(position.Value.x) + (int)math.round(position.Value.y) * boardSize.y,
                    Direction = direction
                });
            }
            
        }).ScheduleParallel();
    }
}