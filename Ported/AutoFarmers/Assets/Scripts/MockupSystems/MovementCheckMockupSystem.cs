using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Transforms;
using UnityEngine;

public class MovementCheckMockupSystem : SystemBase
{
    protected override void OnUpdate()
    {
        var deltaTime = Time.DeltaTime;
        Entities
            .WithStructuralChanges()
            .WithAll<Farmer_Tag>()
            .WithNone<ReadyToMove_Tag>()
            .ForEach((Entity entity, ref MovementTimerMockup timer) =>
            {
                if (timer.Value > 1.0f)
                {
                    EntityManager.AddComponent<ReadyToMove_Tag>(entity);
                }
                else
                {
                    timer.Value += deltaTime * 5;
                }
            }).Run();
    }
}
