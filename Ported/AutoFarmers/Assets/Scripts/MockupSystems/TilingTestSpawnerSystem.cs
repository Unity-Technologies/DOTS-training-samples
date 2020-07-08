using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

public class TilingTestSpawnerSystem : SystemBase
{
    protected override void OnUpdate()
    {
        Entities
            .WithStructuralChanges()
            .ForEach((Entity entity, in TilingTestSpawner spawner) =>
            {
                // Instantiate a spawner
                var farmer = EntityManager.Instantiate(spawner.Farmer);
                SetComponent(farmer, new Translation
                {
                    Value = new float3(0, 0, 0)
                });
                EntityManager.AddComponent<Farmer_Tag>(farmer);
                EntityManager.AddComponent<MovementTimerMockup>(farmer);
                SetComponent(farmer, new MovementTimerMockup
                {
                    Value = 0.0f
                });

                
                EntityManager.DestroyEntity(entity);
            }).Run();
    }
}
