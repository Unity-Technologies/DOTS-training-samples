using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

[UpdateInGroup(typeof(ThrowerArmsGroupSystem))]
[UpdateAfter(typeof(PhysicsSystem))]
public class CollisionSystem : JobComponentSystem
{
    EntityQuery m_group;
    private EntityQuery m_RocksQuery;

    [BurstCompile(FloatMode = FloatMode.Fast)]
    struct CollisionSystemJob : IJobForEachWithEntity<Physics, Translation>
    {
        [ReadOnly]
        public NativeArray<float3> rocks;
        [ReadOnly]
        public NativeArray<Entity> rocksEntities;
        public NativeHashMap<Entity, float3>.ParallelWriter resultCollisionRock;

        public void Execute(Entity entity, int index, ref Physics physics, [ReadOnly] ref Translation translation)
        {
            if (physics.flying)
                return;

            float canDistance;
            float3 rockPos;
            Entity rockEntity;
            NearestPosition(ref rocks, ref rocksEntities, ref translation.Value, out canDistance, out rockPos, out rockEntity);

            // TODO: expose radius
            if (canDistance < 1.0)
            {
                physics.flying = true;
                float3 impactVec = math.normalize(translation.Value - rockPos);
                physics.velocity += impactVec * 10;
                physics.angularVelocity += math.normalize(physics.velocity) * 2;

                resultCollisionRock.TryAdd(rockEntity, impactVec);
            }
        }

        void NearestPosition(ref NativeArray<float3> rocks, ref NativeArray<Entity> entities, ref float3 position, out float nearestDistance, out float3 nearestPos, out Entity resultEntity)
        {
            nearestDistance = math.lengthsq(position - rocks[0]);
            nearestPos = rocks[0];
            resultEntity = entities[0];

            for (int i = 1; i < rocks.Length; i++)
            {
                var targetPosition = rocks[i];
                var distance = math.lengthsq(position - targetPosition);
                var nearest = distance < nearestDistance;

                nearestDistance = math.select(nearestDistance, distance, nearest);
                nearestPos = math.select(nearestPos, targetPosition, nearest);
                if (nearest)
                    resultEntity = entities[i];
            }
            nearestDistance = math.sqrt(nearestDistance);
        }
    }

    [BurstCompile]
    struct CopyRocks : IJobForEachWithEntity<Translation, Physics>
    {
        public float3 boundsMin;
        public float3 boundsMax;
        public NativeHashMap<Entity, float3>.ParallelWriter hashMap;

        public void Execute(Entity entity, int index, [ReadOnly] ref Translation translation, [ReadOnly] ref Physics physics)
        {
            bool inRange = physics.flying;
            inRange = inRange && (translation.Value.z > boundsMin.z);
            inRange = inRange && (translation.Value.z < boundsMax.z);

            if (inRange)
                hashMap.TryAdd(entity, translation.Value);
        }
    }

    protected override JobHandle OnUpdate(JobHandle inputDeps)
    {
        var rockCount = m_RocksQuery.CalculateEntityCount();
        if (rockCount == 0)
            return inputDeps;

        var rocks = new NativeHashMap<Entity, float3>(rockCount, Allocator.TempJob);

        var copyJob = new CopyRocks
        {
            hashMap = rocks.AsParallelWriter(),

            // TODO: remove hardcoded values
            boundsMin = SceneParameters.Instance.TinCanSpawnBoxMin - new Vector3(1, 1, 0.5f),
            boundsMax = SceneParameters.Instance.TinCanSpawnBoxMax + new Vector3(1, 1, 0.5f)
        };

        var jobHandle = copyJob.Schedule(m_RocksQuery, inputDeps);
        jobHandle.Complete();

        var rocksNativeArray = rocks.GetValueArray(Allocator.TempJob);
        var rocksEntityNativeArray = rocks.GetKeyArray(Allocator.TempJob);
        if (rocksNativeArray.Length > 0)
        {
            var rocksThatHit = new NativeHashMap<Entity, float3>(rockCount, Allocator.TempJob);
            var job = new CollisionSystemJob()
            {
                rocks = rocksNativeArray,
                rocksEntities = rocksEntityNativeArray,
                resultCollisionRock = rocksThatHit.AsParallelWriter(),
            };

            jobHandle = job.Schedule(m_group, jobHandle);
            jobHandle.Complete();

            NativeArray<Entity> rockentites = rocksThatHit.GetKeyArray(Allocator.Temp);
            for (int i = 0; i < rockentites.Length; i++)
            {
                Physics p = EntityManager.GetComponentData<Physics>(rockentites[i]);
                p.velocity = rocksThatHit[rockentites[i]] * -1 * 5f;
                EntityManager.SetComponentData(rockentites[i], p);
            }

            rockentites.Dispose();
            rocksThatHit.Dispose();
        }

        rocks.Dispose();
        rocksNativeArray.Dispose();
        rocksEntityNativeArray.Dispose();


        return jobHandle;
    }

    protected override void OnCreate()
    {
        m_group = GetEntityQuery(ComponentType.ReadOnly<TinCanTag>(), ComponentType.ReadWrite<Physics>(), ComponentType.ReadOnly<Translation>());

        m_RocksQuery = GetEntityQuery(new EntityQueryDesc
        {
            All = new[] { ComponentType.ReadOnly<RockTag>(), ComponentType.ReadOnly<Translation>(), ComponentType.ReadWrite<Physics>() },
        });
    }
}
