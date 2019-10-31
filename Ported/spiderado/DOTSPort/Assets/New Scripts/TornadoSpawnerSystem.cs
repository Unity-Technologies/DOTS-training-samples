using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using static Unity.Mathematics.math;
using UnityEngine;

public class TornadoSpawnerSystem : ComponentSystem
{
    protected override void OnUpdate()
    {
        Entities.ForEach((Entity e, ref Spawner spawner, ref Translation translation) =>
        {
            var count = spawner.particleCount;

            var entities = new NativeArray<Entity>(count, Allocator.Temp);
            for (int i = 0; i < count; ++i)
            {
                entities[i] = PostUpdateCommands.Instantiate(spawner.particlePrefab);
            }

            for (int i = 0; i < count; i++)
            {
                float3 pos = new float3(UnityEngine.Random.Range(-50f,50f), UnityEngine.Random.Range(0f,50f),UnityEngine.Random.Range(-50f,50f));
                PostUpdateCommands.SetComponent(entities[i], new Translation { Value = pos });
                // PostUpdateCommands.AddComponent(entities[i], new Point { pos=pos, oldPos=pos, anchor=false, neighborCount=0 });
                // PostUpdateCommands.AddComponent(entities[i], new PartData {
                //     radiusMult = UnityEngine.Random.value,
                //     color = UnityEngine.Color.white * UnityEngine.Random.Range(0.3f, 0.7f),
                //     matrix = UnityEngine.Matrix4x4.TRS(pos, UnityEngine.Quaternion.identity, UnityEngine.Vector3.one * UnityEngine.Random.Range(0.2f, 0.7f))
                // });
                // PostUpdateCommands.AddComponent(entities[i], tornadoData);
                
            }

            PostUpdateCommands.RemoveComponent<Spawner>(e);

            entities.Dispose();
        });
    }
}