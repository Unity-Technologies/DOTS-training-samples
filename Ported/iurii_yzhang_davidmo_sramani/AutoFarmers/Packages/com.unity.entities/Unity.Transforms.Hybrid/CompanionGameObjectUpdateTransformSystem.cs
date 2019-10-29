#if !UNITY_DISABLE_MANAGED_COMPONENTS
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Transforms;
using UnityEngine;
using UnityEngine.Jobs;

struct CompanionGameObjectUpdateTransformSystemState : ISystemStateComponentData { }

[UpdateAfter(typeof(TransformSystemGroup))]
public class CompanionGameObjectUpdateTransformSystem : JobComponentSystem
{
    NativeArray<Entity> m_Entities;
    TransformAccessArray m_TransformAccessArray;

    EntityQuery m_NewQuery;
    EntityQuery m_ExistingQuery;
    EntityQuery m_DestroyedQuery;

    protected override void OnCreate()
    {
        m_Entities = new NativeArray<Entity>(0, Allocator.Persistent);
        m_TransformAccessArray = new TransformAccessArray(0);

        m_NewQuery = GetEntityQuery(
            new EntityQueryDesc
            {
                All = new[] { ComponentType.ReadOnly<CompanionLink>() },
                None = new[] { ComponentType.ReadOnly<CompanionGameObjectUpdateTransformSystemState>() }
            }
        );

        m_ExistingQuery = GetEntityQuery(
            new EntityQueryDesc
            {
                All = new[] { ComponentType.ReadOnly<CompanionLink>(), ComponentType.ReadOnly<CompanionGameObjectUpdateTransformSystemState>() }
            }
        );

        m_DestroyedQuery = GetEntityQuery(
            new EntityQueryDesc
            {
                All = new[] { ComponentType.ReadOnly<CompanionGameObjectUpdateTransformSystemState>() },
                None = new[] { ComponentType.ReadOnly<CompanionLink>() }
            }
        );
    }

    protected override void OnDestroy()
    {
        m_Entities.Dispose();
        m_TransformAccessArray.Dispose();
    }

    protected override JobHandle OnUpdate(JobHandle inputDeps)
    {
        if (!m_DestroyedQuery.IsEmptyIgnoreFilter || !m_NewQuery.IsEmptyIgnoreFilter)
        {
            EntityManager.AddComponent<CompanionGameObjectUpdateTransformSystemState>(m_NewQuery);
            EntityManager.RemoveComponent<CompanionGameObjectUpdateTransformSystemState>(m_DestroyedQuery);

            m_Entities.Dispose();
            m_Entities = m_ExistingQuery.ToEntityArray(Allocator.Persistent);

            var transforms = new Transform[m_Entities.Length];
            for (int i = 0; i < m_Entities.Length; i++)
                transforms[i] = EntityManager.GetComponentObject<CompanionLink>(m_Entities[i]).transform;
            m_TransformAccessArray.SetTransforms(transforms);
        }

        return new CopyTransformJob
        {
            localToWorld = GetComponentDataFromEntity<LocalToWorld>(),
            entities = m_Entities
        }.Schedule(m_TransformAccessArray, inputDeps);
    }

    [BurstCompile]
    struct CopyTransformJob : IJobParallelForTransform
    {
        [NativeDisableParallelForRestriction]
        public ComponentDataFromEntity<LocalToWorld> localToWorld;
        [ReadOnly]
        public NativeArray<Entity> entities;

        public void Execute(int index, TransformAccess transform)
        {
            var ltw = localToWorld[entities[index]];
            transform.position = ltw.Position;
            transform.rotation = ltw.Rotation;
        }
    }
}
#endif
