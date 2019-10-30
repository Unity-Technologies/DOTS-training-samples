using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Rendering;
using Unity.Transforms;
using UnityEngine;
using UnityEngine.Rendering;

[UpdateInGroup(typeof(InitializationSystemGroup))]
public class SphereSpawner : JobComponentSystem
{
    struct ExecuteOnceTag : IComponentData {}
    
    [BurstCompile]
    struct Spawn : IJobParallelFor
    {
        public EntityCommandBuffer.Concurrent ec;
        public Entity prefab;
        public Unity.Mathematics.Random random;
        
        public void Execute(int index)
        {
            var e = ec.Instantiate(index, prefab);
            var p = random.NextFloat3Direction();
            ec.SetComponent(index, e, new Translation{ Value = p * 2.0f });
            ec.SetComponent(index, e, new MoveSpeed{ Value = p * 0.1f });
        }
    }

    private Entity m_spherePrefabEntity;
    private Entity m_initEntity;
    
    protected override void OnCreate()
    {
        if (SphereSpawnerUnity.instance != null)
        {
            m_initEntity = EntityManager.CreateEntity();
            EntityManager.AddComponent<ExecuteOnceTag>(m_initEntity);
            
            var spherePrefabArchetype = EntityManager.CreateArchetype(typeof(Translation), typeof(LocalToWorld), typeof(MoveSpeed), typeof(RenderMesh));

            m_spherePrefabEntity = EntityManager.CreateEntity(spherePrefabArchetype);
            EntityManager.SetSharedComponentData(m_spherePrefabEntity, new RenderMesh
            {
                mesh = SphereSpawnerUnity.instance.prefabObject.GetComponent<MeshFilter>().sharedMesh,
                material = SphereSpawnerUnity.instance.prefabObject.GetComponent<Renderer>().sharedMaterial,
                castShadows = ShadowCastingMode.On,
                receiveShadows = true
            });
        }

        var query = GetEntityQuery(ComponentType.ReadOnly<ExecuteOnceTag>());
        RequireForUpdate(query);
    }
    
    protected override JobHandle OnUpdate(JobHandle inputDeps)
    {
        EntityManager.DestroyEntity(m_initEntity);
        m_initEntity = Entity.Null;

        var ecbSystem = World.GetOrCreateSystem<EndInitializationEntityCommandBufferSystem>();
        var ecb = ecbSystem.CreateCommandBuffer();
        
        var sp = new Spawn
        {
            ec = ecb.ToConcurrent(),
            prefab = m_spherePrefabEntity,
            random = new Unity.Mathematics.Random(123)
        };

        var handle = sp.Schedule(1024, 128, inputDeps);
        ecbSystem.AddJobHandleForProducer(handle);
 
        Debug.Log("SphereSpawner");
        
        return handle;
    }
}