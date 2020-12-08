using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Collections;
using Unity.Transforms;
using Unity.Mathematics;
using Unity.Rendering;

public class BeeSpawnerSystem : SystemBase
{
    protected override void OnUpdate()
    {
        var ecb = new EntityCommandBuffer(Allocator.Temp);
        var random = new Unity.Mathematics.Random(1234);

        Entities
            .ForEach((Entity entity, in BeeSpawner spawner) =>
            {
                // Destroying the current entity is a classic ECS pattern,
                // when something should only be processed once then forgotten.
                ecb.DestroyEntity(entity);

                // spawn bees
                for (int i = 0; i < spawner.numBeesToSpawn; ++i)
                {
                    
                    var bee = ecb.Instantiate(spawner.beePrefab);

                    // give the bee a position near the spawn point
                    float3 randomDistance = random.NextFloat3(spawner.radius);
                    var translation = new Translation { Value = (spawner.position + randomDistance) };
                    ecb.SetComponent(bee, translation);

                    //give the bee the team colour
                    ecb.SetComponent(bee, new URPMaterialPropertyBaseColor
                    {
                        Value = spawner.teamColour
                    });

                    // assign the Bee component with team id
                    ecb.SetComponent(bee, new Bee
                    {
                        teamID = spawner.teamNumber
                    });
                }
            }).Run();

        ecb.Playback(EntityManager);
        ecb.Dispose();
    }
}
