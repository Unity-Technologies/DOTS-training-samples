using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
using Random = Unity.Mathematics.Random;

public class InitilizationSystem : SystemBase
{
    protected override void OnUpdate()
    {
        var ecb = new EntityCommandBuffer(Allocator.Temp);
        
        var random = new Random(6541);
        
        Entities
            .ForEach((Entity entity, in Init init) =>
            {
                
                ecb.DestroyEntity(entity);

                for (int i = 0; i < init.antCount; i++)
                {
                    var ant = ecb.Instantiate(init.antPrefab);
                    var translation = new Translation{Value = new float3(64 + random.NextFloat(-10,10),64 + random.NextFloat(-10,10), 0)};
                    ecb.SetComponent(ant, translation);
                    
                    
                    
                    
                }
            }).Run();
    }
}
