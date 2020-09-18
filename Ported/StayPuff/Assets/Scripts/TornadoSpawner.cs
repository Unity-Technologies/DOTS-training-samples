using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
//using UnityEngine;
[UpdateInGroup(typeof(InitializationSystemGroup))]
public class TornadoSpawner : SystemBase
{
    Random SystemRandom;

    protected override void OnCreate()
    {
        SystemRandom = new Random(999);
    }

    protected override void OnUpdate()
    {
        Random jobRandom = SystemRandom;
		var ecb = new EntityCommandBuffer(Allocator.TempJob);
        
        Entities
            .ForEach((Entity entity, in TornadoSpawnData tornadoSpawnData,in LocalToWorld ltw, in TornadoPositionData tornadoPositionData) =>
            {
          
                for (int x = 0; x < tornadoSpawnData.TornadoCount; ++x)
                {

                    var posX = jobRandom.NextInt(0, 50);
                    var posY = 0;
                    var posZ = jobRandom.NextInt(0, 50);
                    var instance = ecb.Instantiate(tornadoSpawnData.Prefab);
                    ecb.SetComponent(instance, new Translation
                    {
                        Value = new float3(posX, 0, posZ)
                    });
                    
					ecb.AddComponent(instance,new TornadoPositionData
                    {
                        Position = new float3(posX, 0, posZ)
                    });

                }

                ecb.DestroyEntity(entity);
            }).Run();
		ecb.Playback(EntityManager);
		ecb.Dispose();
    }
}