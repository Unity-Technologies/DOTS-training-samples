using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

public class MovementMockupSystem : SystemBase
{
    protected override void OnUpdate()
    {
        Entities
            .WithStructuralChanges()
            .WithAll<ReadyToMove_Tag>()
            .WithAll<Farmer_Tag>()
            .WithNone<PathFindingTargetReached_Tag>()
            .ForEach((Entity entity, ref Translation translation, in TillRect tillRect) =>
            {
                int PosX = (int)(translation.Value.x);
                int PosY = (int)(translation.Value.z);

                // teleport to first one
                if (PosX < tillRect.X || PosY < tillRect.Y || PosX >= tillRect.X + tillRect.Width || PosY >= tillRect.Y + tillRect.Height)
                {
                    PosX = tillRect.X;
                    PosY = tillRect.Y;
                }
                
                // just teleport to next one
                else if ((PosX - tillRect.X) % tillRect.Width == tillRect.Width - 1)
                {
                    PosX = tillRect.X;
                    PosY++;
                }
                else
                {
                    PosX++;
                }
                
                translation.Value = new float3(PosX, 0.5f, PosY);
                EntityManager.RemoveComponent<ReadyToMove_Tag>(entity);
                EntityManager.AddComponent<PathFindingTargetReached_Tag>(entity);
                SetComponent(entity, new MovementTimerMockup
                {
                    Value = 0.0f
                });
            }).Run();
    }
}
