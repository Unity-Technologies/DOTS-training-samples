using System;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Rendering;
using Unity.Transforms;
using UnityEngine;

namespace Systems
{
    [UpdateInGroup(typeof(InitializationSystemGroup))]
    public class CreateIntersectionsSystem : JobComponentSystem
    {
        EntityQuery m_RoadSetupQuery;
        EntityArchetype m_IntersectionArchetype;
        EntityQuery m_IntersectionQuery;

        protected override void OnCreate()
        {
            base.OnCreate();
            m_RoadSetupQuery = GetEntityQuery(typeof(RoadSetupComponent));
            m_RoadSetupQuery.SetChangedVersionFilter(typeof(RoadSetupComponent));
            m_IntersectionArchetype = EntityManager.CreateArchetype(
                typeof(IntersectionComponent),
                typeof(RenderMesh),
                typeof(LocalToWorld),
                typeof(Static)
            );
            m_IntersectionQuery = GetEntityQuery(typeof(IntersectionComponent), typeof(RenderMesh));
        }

        protected override JobHandle OnUpdate(JobHandle inputDeps)
        {
            bool hasChanged = m_RoadSetupQuery.CalculateEntityCount() > 0;
            if (!hasChanged)
                return default;
            var roadsEntity = m_RoadSetupQuery.GetSingletonEntity();
            var roads = EntityManager.GetComponentData<RoadSetupComponent>(roadsEntity);
            var numIntersections = roads.Intersections.Value.Intersections.Length;

            var entities = new NativeArray<Entity>(numIntersections, Allocator.Temp);
            EntityManager.CreateEntity(m_IntersectionArchetype, entities);

            var meshData = EntityManager.GetSharedComponentData<RenderMesh>(roadsEntity);

            // this crashes Unity:
            // EntityManager.SetSharedComponentData(m_IntersectionQuery, meshData);
            for (int i = 0; i < entities.Length; i++)
                EntityManager.SetSharedComponentData(entities[i], meshData);

            var scale = new float3(roads.IntersectionSize, roads.IntersectionSize, roads.TrackThickness);
            var blob = roads.Intersections;
            return Entities.WithStoreEntityQueryInField(ref m_IntersectionQuery).ForEach((int entityInQueryIndex, ref IntersectionComponent intersection, ref LocalToWorld ltw) =>
            {
                ref var entry = ref blob.Value.Intersections[entityInQueryIndex];
                ltw.Value = float4x4.TRS(entry.Position, Quaternion.LookRotation(entry.Normal), scale);
                intersection.Index = (ushort)entityInQueryIndex;
            }).Schedule(inputDeps);
        }
    }
}
