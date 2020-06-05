using System;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;

[UpdateInGroup(typeof(PresentationSystemGroup))]
public class CarSortingByLaneSystem : SystemBase
{
    public struct CarInfo : IComparable<CarInfo>
    {
        public float position;
        public float speed;

        public int CompareTo(CarInfo other)
        {
            return position.CompareTo(other.position);
        }
    }
    public struct Database
    {
        public UnsafeListWrapper<UnsafeListWrapper<CarInfo>> m_CarInfosByTrackGroupIdx;
        public float m_TrackLength;

        public CarInfo GetCarInFront(float lane, float ownProgress)
        {
            if (!m_CarInfosByTrackGroupIdx.IsCreated)
            {
                return new CarInfo
                {
                    position = float.MaxValue,
                    speed = float.MaxValue
                };
            }

            int laneGroup = TrackGroup.LaneValueToTrackGroupIdx(lane);

            CarInfo entryWithMinProgressInFront = new CarInfo { position = float.MaxValue, speed = float.MaxValue };
            float minDistanceInFrontOfUs = float.MaxValue;

            GetEntryWithMinProgressInFront(laneGroup, ownProgress, ref minDistanceInFrontOfUs, ref entryWithMinProgressInFront);

            if (laneGroup > 0)
            {
                GetEntryWithMinProgressInFront(laneGroup - 1, ownProgress, ref minDistanceInFrontOfUs, ref entryWithMinProgressInFront);
            }

            if (laneGroup < m_CarInfosByTrackGroupIdx.Length - 1)
            {
                GetEntryWithMinProgressInFront(laneGroup + 1, ownProgress, ref minDistanceInFrontOfUs, ref entryWithMinProgressInFront);
            }

            return entryWithMinProgressInFront;
        }

        void GetEntryWithMinProgressInFront(int laneGroup, float ownProgress, ref float minDistanceInFront, ref CarInfo entryWithMinProgressInFront)
        {
            var carInfos = m_CarInfosByTrackGroupIdx[laneGroup];
            if (carInfos.Length > 0)
            {
                var found = ArrayBinarySearch(carInfos, ownProgress, out var indexInData);
                if (!found || carInfos.Length > 1)
                {
                    var entryInFront = carInfos[(indexInData + 1) % carInfos.Length];

                    var distanceInFront = TrackPosition.GetLoopedDistanceInFront(ownProgress, entryInFront.position, m_TrackLength);

                    if (distanceInFront < minDistanceInFront)
                    {
                        entryWithMinProgressInFront = entryInFront;
                        minDistanceInFront = distanceInFront;
                    }
                }
            }
        }

        public bool GetCarOnAdjacentLane(float lane, int offset, float ownProgress, out CarInfo carInFront, out CarInfo carBehind)
        {
            carInFront = new CarInfo
            {
                position = float.MaxValue,
                speed = float.MaxValue
            };

            carBehind = new CarInfo
            {
                position = float.MaxValue,
                speed = float.MaxValue
            };

            if (!m_CarInfosByTrackGroupIdx.IsCreated)
            {
                return false;
            }

            int laneGroup = TrackGroup.LaneValueToTrackGroupIdx(lane);

            float minDistanceInFrontOfUs = float.MaxValue;
            float minDistanceBehindUs = float.MaxValue;

            var laneGroupMostFarAway = laneGroup + offset + offset;
            if (laneGroupMostFarAway >= 0 && laneGroupMostFarAway <= m_CarInfosByTrackGroupIdx.Length - 1)
            {
                GetEntryInFrontAndBehind(laneGroupMostFarAway - offset, ownProgress,
                    ref minDistanceInFrontOfUs, ref carInFront,
                    ref minDistanceBehindUs, ref carBehind);

                GetEntryInFrontAndBehind(laneGroupMostFarAway, ownProgress,
                    ref minDistanceInFrontOfUs, ref carInFront,
                    ref minDistanceBehindUs, ref carBehind);

                return true;
            }

            return false;
        }

        void GetEntryInFrontAndBehind(int laneGroup, float ownProgress, 
            ref float minDistanceInFront, ref CarInfo entryWithMinDistanceInFront,
            ref float minDistanceBehind, ref CarInfo entryWithMinDistanceBehind)
        {
            var carInfos = m_CarInfosByTrackGroupIdx[laneGroup];
            if (carInfos.Length > 0)
            {
                var found = ArrayBinarySearch(carInfos, ownProgress, out var entryIdxBehind);
                if (!found || carInfos.Length > 1)
                {
                    var entryInFront = carInfos[(entryIdxBehind + 1) % carInfos.Length];

                    var distanceInFront = TrackPosition.GetLoopedDistanceInFront(ownProgress, entryInFront.position, m_TrackLength);

                    if (distanceInFront < minDistanceInFront)
                    {
                        entryWithMinDistanceInFront = entryInFront;
                        minDistanceInFront = distanceInFront;
                    }

                    var entryBehind = carInfos[(entryIdxBehind) % carInfos.Length];

                    var distanceBehind = TrackPosition.GetLoopedDistanceInFront(entryBehind.position, ownProgress, m_TrackLength);

                    if (distanceBehind < minDistanceBehind)
                    {
                        entryWithMinDistanceBehind = entryBehind;
                        minDistanceBehind = distanceBehind;
                    }
                }
            }
        }

        // return true if search is successful, false otherwise
        // indexInData is index of entry having same value as item if search is successful
        // if search is unsuccessful, indexInData is index of largest entry that is smaller than item
        static bool ArrayBinarySearch(in UnsafeListWrapper<CarInfo> sortedArrayToSearch, float item, out int indexInData)
        {
            int min = 0;
            int N = sortedArrayToSearch.Length;
            int max = N - 1;
            do
            {
                int mid = (min + max) / 2;

                if (item > sortedArrayToSearch[mid].position)
                    min = mid + 1;
                else
                    max = mid - 1;

                if (sortedArrayToSearch[mid].position == item)
                {
                    indexInData = mid;
                    return true;
                }

            } while (min <= max);

            indexInData = max;
            return false;
        }
    }
    Database m_Database;

    public Database GetDatabase()
    {
        return m_Database;
    }

    protected override void OnUpdate()
    {
        var monitorFrontSystem = World.GetExistingSystem<MonitorFrontSystemV2>();
        var readerJobHandleA = monitorFrontSystem.GetJobHandleReadFromCarInfos();

        var laneChangeSystem = World.GetExistingSystem<LaneChangeSystem>();
        var readerJobHandleB = laneChangeSystem.GetJobHandleReadFromCarInfos();

        var inputDep = JobHandle.CombineDependencies(Dependency, readerJobHandleA, readerJobHandleB);

        var trackProperties = GetSingleton<TrackProperties>();
        var numOfTrackGroup = trackProperties.NumberOfLanes * 2 - 1;

        if (!m_Database.m_CarInfosByTrackGroupIdx.IsCreated)
        {
            m_Database.m_CarInfosByTrackGroupIdx = new UnsafeListWrapper<UnsafeListWrapper<CarInfo>>(numOfTrackGroup, Allocator.Persistent);

            for (int i = 0; i < numOfTrackGroup; ++i)
            {
                m_Database.m_CarInfosByTrackGroupIdx.Add(new UnsafeListWrapper<CarInfo>(64, Allocator.Persistent));
            }

            m_Database.m_TrackLength = trackProperties.TrackLength;
        }

        var jobHandles = new NativeArray<JobHandle>(numOfTrackGroup, Allocator.Temp);

        for (int i = 0; i < numOfTrackGroup; ++i)
        {
            var carInfosToFill = m_Database.m_CarInfosByTrackGroupIdx[i];

            var jobHandle = Job.WithCode(() =>
            {
                carInfosToFill.Clear();
            })
            .Schedule(inputDep);

            jobHandle = Entities
                .WithSharedComponentFilter(new TrackGroup { Index = i })
                .ForEach((in TrackPosition trackPos, in Speed speed) =>
            {
                carInfosToFill.Add(new CarInfo
                {
                    position = trackPos.TrackProgress,
                    speed = speed.Value
                });
            })
            .Schedule(jobHandle);

            jobHandles[i] = Job.WithCode(() =>
            {
                var carInfosAsNativeArray = carInfosToFill.AsArray();

#if ENABLE_UNITY_COLLECTIONS_CHECKS
                var tempSafetyHandle = AtomicSafetyHandle.Create();
                NativeArrayUnsafeUtility.SetAtomicSafetyHandle(ref carInfosAsNativeArray, tempSafetyHandle);
#endif

                carInfosAsNativeArray.Sort();

#if ENABLE_UNITY_COLLECTIONS_CHECKS
                AtomicSafetyHandle.Release(tempSafetyHandle);
#endif
            })
            .Schedule(jobHandle);
        }

        Dependency = JobHandle.CombineDependencies(jobHandles);

        jobHandles.Dispose();
    }

    protected override void OnDestroy()
    {
        if (m_Database.m_CarInfosByTrackGroupIdx.IsCreated)
        {
            for (int i = 0; i < m_Database.m_CarInfosByTrackGroupIdx.Length; ++i)
            {
                m_Database.m_CarInfosByTrackGroupIdx[i].Dispose();
            }
            m_Database.m_CarInfosByTrackGroupIdx.Dispose();
        }
    }
}
