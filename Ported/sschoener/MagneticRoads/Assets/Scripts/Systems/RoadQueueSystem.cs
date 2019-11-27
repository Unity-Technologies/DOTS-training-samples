using System;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using UnityEngine.Rendering;

namespace Systems {

    public struct QueueData
    {
        public int Offset;
        public int Length;

        public int Begin => Offset;
        public int End => Offset + Length;
    }

    [UpdateInGroup(typeof(InitializationSystemGroup))]
    public class RoadQueueSystem : JobComponentSystem
    {
        public NativeArray<QueueEntry> QueueEntries;
        public NativeArray<QueueData> Queues;
        public NativeArray<BitArray8> IntersectionOccupation;
        EntityQuery m_RoadSetupQuery;

        public static int GetQueueIndex(SplinePosition splinePos) =>
            4 * splinePos.Spline + splinePos.Direction + 1 + (splinePos.Side + 1) / 2;
        
        void ClearArrays()
        {
            if (Queues.IsCreated)
                Queues.Dispose();
            if (QueueEntries.IsCreated)
                QueueEntries.Dispose();
            if (IntersectionOccupation.IsCreated)
                IntersectionOccupation.Dispose();
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            ClearArrays();
        }

        protected override JobHandle OnUpdate(JobHandle inputDeps)
        {
            m_RoadSetupQuery = GetEntityQuery(typeof(RoadSetupComponent));
            m_RoadSetupQuery.SetChangedVersionFilter(typeof(RoadSetupComponent));
            bool hasChanged = m_RoadSetupQuery.CalculateEntityCount() > 0;
            if (!hasChanged)
                return default;
            var roadsEntity = m_RoadSetupQuery.GetSingletonEntity();
            var roads = EntityManager.GetComponentData<RoadSetupComponent>(roadsEntity);

            ClearArrays();
            IntersectionOccupation = new NativeArray<BitArray8>(
                1 + roads.Intersections.Value.Intersections.Length/4,
                Allocator.Persistent
            );
            
            Queues = new NativeArray<QueueData>(roads.Splines.Value.Splines.Length * 4, Allocator.Persistent);
            int totalSize = 0;

            var localQueues = Queues;
            Job.WithCode(() =>
            {
                ref var splines = ref roads.Splines.Value.Splines;
                for (int i = 0; i < splines.Length; i++)
                {
                    int size = 1 + splines[i].MaxCarCount;
                    for (int q = 0; q < 4; q++)
                    {
                        localQueues[4 * i + q] = new QueueData
                        {
                            Length = 0,
                            Offset = totalSize
                        };
                        totalSize += size;
                    }
                }
            }).Run();
            QueueEntries = new NativeArray<QueueEntry>(totalSize, Allocator.Persistent);
            return default;
        }
    }
}
