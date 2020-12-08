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
        EntityCommandBufferSystem sys = World.GetExistingSystem<EndSimulationEntityCommandBufferSystem>();
        var ecb = sys.CreateCommandBuffer();
        var random = new Unity.Mathematics.Random(1234);

        Entities
            .ForEach((Entity entity, ref BeeSpawnRequest req, in BeeSpawner spawner) =>
            {

                // spawn bees
                for (int i = 0; i < req.numOfBeesToSpawn; ++i)
                {
                    
                    var bee = ecb.Instantiate(spawner.beePrefab);

                    // give the bee a position near the spawn point
                    float3 randomDistance = random.NextFloat3(-spawner.radius, spawner.radius);
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

                // remove the spawn request prefab once done
                ecb.RemoveComponent<BeeSpawnRequest>(entity);
                
            }).Run();

        sys.AddJobHandleForProducer(Dependency);
       
    }
}
