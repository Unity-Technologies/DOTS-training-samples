using System.Collections;
using System.Collections.Generic;
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
public class SphereSpawner : ComponentSystem
{
    struct SphereSpawnerTag : IComponentData {}
    
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
        // This will crash Unity
        //spherePrefabEntity =
        //    GameObjectConversionUtility.ConvertGameObjectHierarchy(SphereSpawnerUnity.instance.prefabObject,
        //        World);

        if (SphereSpawnerUnity.instance != null)
        {
            m_initEntity = EntityManager.CreateEntity();
            EntityManager.AddComponent<SphereSpawnerTag>(m_initEntity);
        }

        var query = GetEntityQuery(ComponentType.ReadOnly<SphereSpawnerTag>());
        RequireForUpdate(query);
    }

    protected override void OnUpdate()
    {   
        var spherePrefabArchetype = EntityManager.CreateArchetype(typeof(Translation), typeof(LocalToWorld), typeof(MoveSpeed), typeof(RenderMesh));

        m_spherePrefabEntity = EntityManager.CreateEntity(spherePrefabArchetype);
        EntityManager.SetSharedComponentData(m_spherePrefabEntity, new RenderMesh
        {
            mesh = SphereSpawnerUnity.instance.prefabObject.GetComponent<MeshFilter>().sharedMesh,
            material = SphereSpawnerUnity.instance.prefabObject.GetComponent<Renderer>().sharedMaterial,
            castShadows = ShadowCastingMode.On,
            receiveShadows = true
        });
        
        var ecb = new EntityCommandBuffer(Allocator.TempJob);
        var sp = new Spawn
        {
            ec = ecb.ToConcurrent(),
            prefab = m_spherePrefabEntity,
            random = new Unity.Mathematics.Random(0xBAD5EED)
        };
        
        sp.Schedule(1024, 64).Complete();
        ecb.Playback(EntityManager);
        ecb.Dispose();
        
        EntityManager.DestroyEntity(m_initEntity);
        m_initEntity = Entity.Null;
    }
}