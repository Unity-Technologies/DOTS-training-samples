using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Rendering;
using Unity.Transforms;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Rendering;

[UpdateInGroup(typeof(InitializationSystemGroup))]
public class SphereSpawner : JobComponentSystem
{
    struct ExecuteOnceTag : IComponentData {}
    private EntityQuery m_executeOnce;

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
    
    protected override void OnCreate()
    {
        if (SphereSpawnerUnity.instance != null)
        {
            ResetExecuteOnceTag(EntityManager);
            
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

        m_executeOnce = GetEntityQuery(ComponentType.ReadOnly<ExecuteOnceTag>());
        RequireForUpdate(m_executeOnce);
    }
    
    protected override JobHandle OnUpdate(JobHandle inputDeps)
    {
        var once = m_executeOnce.GetSingletonEntity();
        Assert.IsTrue(once != Entity.Null);
        EntityManager.DestroyEntity(once);
        
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
    
    public static void ResetExecuteOnceTag(EntityManager mManager)
    {
        var q = mManager.CreateEntityQuery(ComponentType.ReadOnly<ExecuteOnceTag>());
        if (q.CalculateEntityCount() == 0)
        {
            var e = mManager.CreateEntity();
            mManager.AddComponent<ExecuteOnceTag>(e);
        }
    }
}