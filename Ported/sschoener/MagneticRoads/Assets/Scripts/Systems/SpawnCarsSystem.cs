using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Rendering;
using Unity.Transforms;
using UnityEngine;

namespace Systems {
    
    [UpdateInGroup(typeof(InitializationSystemGroup))]
    public class SpawnCarsSystem : JobComponentSystem
    {
        EntityArchetype m_CarArchetype;
        EntityQuery m_NewCarsQuery;
        EntityQuery m_SpawnQuery;
        EntityQuery m_CarSetupQuery;
        
        protected override void OnCreate()
        {
            base.OnCreate();
            m_CarArchetype = EntityManager.CreateArchetype(
                typeof(CarSpeedComponent),
                typeof(SplineDataComponent),
                typeof(OnSplineComponent),
                typeof(LocalIntersectionComponent),
                typeof(CoordinateSystemComponent),
                typeof(LocalToWorld),
                typeof(CarColor),
                typeof(UninitializedComponentTag)
            );
            m_NewCarsQuery = GetEntityQuery(typeof(UninitializedComponentTag), typeof(CarSpeedComponent));
            m_CarSetupQuery = GetEntityQuery(typeof(CarSetupTagComponent));
        }

        protected override JobHandle OnUpdate(JobHandle inputDeps)
        {
            int numCars = 0;
            Entities.WithStoreEntityQueryInField(ref m_SpawnQuery).ForEach((in CarSpawnComponent spawn) => numCars += spawn.Count).Run();
            if (numCars == 0)
                return default;
            var entities = new NativeArray<Entity>(numCars, Allocator.Temp);
            EntityManager.CreateEntity(m_CarArchetype, entities);
            
            var setupEntity = m_CarSetupQuery.GetSingletonEntity();
            EntityManager.SetSharedComponentData(m_NewCarsQuery, EntityManager.GetSharedComponentData<RenderMesh>(setupEntity));
            EntityManager.DestroyEntity(m_SpawnQuery);

            int seed = 100271 * UnityEngine.Time.frameCount;
            var roads = GetSingleton<RoadSetupComponent>();
            int numSplines = roads.Splines.Value.Splines.Length;
            Entities.WithAll<UninitializedComponentTag>().ForEach((Entity entity, ref CarSpeedComponent speed, ref OnSplineComponent onSpline, ref CarColor carColor) =>
            {
                var rng = new Unity.Mathematics.Random((uint)((1 + entity.Index) * seed));
                speed.SplineTimer = 1;
                carColor.Value = new float4(rng.NextFloat(), rng.NextFloat(), rng.NextFloat(), 1);
                onSpline.Value = new SplinePosition
                {
                    Direction = (sbyte)(-1 + rng.NextInt(2) * 2),            
                    Side = (sbyte)(-1 + rng.NextInt(2) * 2),
                    Spline = (ushort)rng.NextInt(0, numSplines),                  
                };
            }).Schedule(inputDeps).Complete();
            Entities.WithAll<UninitializedComponentTag>().ForEach((Entity entity, in OnSplineComponent onSpline) =>
            {
                TrackSplines.GetQueue(onSpline.Value).Add(new QueueEntry
                {
                    Entity = entity, SplineTimer = 1
                });
            }).WithoutBurst().Run();
            EntityManager.RemoveComponent<UninitializedComponentTag>(EntityManager.UniversalQuery);
            return default;
        }
    }
}
