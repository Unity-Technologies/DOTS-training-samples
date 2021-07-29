using System;
using src.Components;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace src.Systems
{
    public abstract class BucketWorkerUpdateBase : SystemBase
    {
        protected enum QueryBuckets
        {
            Full,
            Empty
        }

        EndSimulationEntityCommandBufferSystem m_EndSimulationEntityCommandBufferSystem;
        EntityQuery m_BucketQuery;

        protected abstract QueryBuckets WhichBucketsToQuery { get;}

        protected override void OnCreate()
        {
            base.OnCreate();

            switch (WhichBucketsToQuery)
            {
                case QueryBuckets.Full:
                    m_BucketQuery = GetEntityQuery(
                        ComponentType.ReadOnly<EcsBucket>(),
                        ComponentType.ReadOnly<FullBucketTag>(),
                        ComponentType.Exclude<BucketIsHeld>(),
                        ComponentType.Exclude<PickUpBucketRequest>(),
                        ComponentType.Exclude<FillingUpBucketTag>(),
                        ComponentType.Exclude<ThrowBucketAtFire>(),
                        ComponentType.ReadOnly<Position>());
                    break;
                case QueryBuckets.Empty:
                    m_BucketQuery = GetEntityQuery(
                        ComponentType.ReadOnly<EcsBucket>(),
                        ComponentType.Exclude<FullBucketTag>(),
                        ComponentType.Exclude<BucketIsHeld>(),
                        ComponentType.Exclude<PickUpBucketRequest>(),
                        ComponentType.Exclude<FillingUpBucketTag>(),
                        ComponentType.Exclude<ThrowBucketAtFire>(),
                        ComponentType.ReadOnly<Position>());
                    break;
                default:
                    throw new NotImplementedException(WhichBucketsToQuery.ToString());
            }

            m_EndSimulationEntityCommandBufferSystem = World.GetExistingSystem<EndSimulationEntityCommandBufferSystem>();

            RequireSingletonForUpdate<FireSimConfigValues>();
        }

        protected void AddECBAsDependency()
        {
            m_EndSimulationEntityCommandBufferSystem.AddJobHandleForProducer(Dependency);
        }

        protected NativeArray<Position> QueryBucketPositions()
        {
            return m_BucketQuery.ToComponentDataArray<Position>(Allocator.TempJob);
        }

        protected NativeArray<EcsBucket> QueryBucketDatas()
        {
            return m_BucketQuery.ToComponentDataArray<EcsBucket>(Allocator.TempJob);
        }

        protected NativeArray<Entity> QueryBucketEntities()
        {
            return m_BucketQuery.ToEntityArray(Allocator.TempJob);
        }

        protected EntityCommandBuffer.ParallelWriter CreateECBParallerWriter()
        {
            var ecb = m_EndSimulationEntityCommandBufferSystem.CreateCommandBuffer();
            return ecb.AsParallelWriter();
        }

        protected static float2 GetPositionInTeam(float2 startPosition, float2 endPosition, int id, int teamCount)
        {
            float t = (float)id / (float)(teamCount - 1);

            var position = math.lerp(startPosition, endPosition, t);
            // Give it some curve
            var curveOffset = math.normalizesafe(endPosition - startPosition) * 10.0f;
            var tmp = curveOffset.x;
            curveOffset.x = -curveOffset.y;
            curveOffset.y = tmp;

            curveOffset = math.sin(t * math.PI) * curveOffset;
            position += curveOffset;

            return position;
        }
    }
}