using System;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;

public class CarSortingByLaneSystem : SystemBase
{
    public struct Entry : IComparable<Entry>
    {
        public float position;
        public float speed;

        public int CompareTo(Entry other)
        {
            return position.CompareTo(other.position);
        }
    }
    public List<NativeList<Entry>> m_CarInfoByLaneGroup;
    public int numLaneGroup;
    public float trackLength;

    protected override void OnDestroy()
    {
        if (m_CarInfoByLaneGroup != null)
        {
            for (int i = 0; i < m_CarInfoByLaneGroup.Count; ++i)
            {
                m_CarInfoByLaneGroup[i].Dispose();
            }
        }
    }

    protected override void OnUpdate()
    {
        var trackProperties = GetSingleton<TrackProperties>();
        var numOfTrackGroup = trackProperties.NumberOfLanes * 2 - 1;

        if (m_CarInfoByLaneGroup == null)
        {
            m_CarInfoByLaneGroup = new List<NativeList<Entry>>(numOfTrackGroup);

            for (int i = 0; i < numOfTrackGroup; ++i)
            {
                m_CarInfoByLaneGroup.Add(new NativeList<Entry>(64, Allocator.Persistent));
            }

            numLaneGroup = numOfTrackGroup;
            trackLength = trackProperties.TrackLength;
        }

        var jobHandles = new NativeArray<JobHandle>(numOfTrackGroup, Allocator.Temp);

        for (int i = 0; i < numOfTrackGroup; ++i)
        {
            var targetList = m_CarInfoByLaneGroup[i];

            var jobHandle = Job.WithCode(() =>
            {
                targetList.Clear();
            })
            .Schedule(Dependency);

            var targetTrackGroup = new TrackGroup();
            targetTrackGroup.index = i;

            jobHandle = Entities
                .WithSharedComponentFilter(targetTrackGroup)
                .ForEach((in TrackPosition trackPos, in Speed speed) =>
            {
                var entryValue = new Entry
                {
                    position = trackPos.TrackProgress,
                    speed = speed.Value
                };
                targetList.Add(entryValue);
            })
            .Schedule(jobHandle);

            jobHandles[i] = Job.WithCode(() =>
            {
                targetList.AsArray().Sort();
            })
            .Schedule(jobHandle);

            // Debug start
            //jobHandles[i].Complete();
            //Job.WithCode(() =>
            //{
            //    UnityEngine.Debug.Log("lane group = " + i);
            //    for (int j = 0; j < targetList.Length; ++j)
            //    {
            //        UnityEngine.Debug.Log("entry idx = " + j);
            //        UnityEngine.Debug.Log("position = " + targetList[j].position);
            //        UnityEngine.Debug.Log("speed = " + targetList[j].speed);
            //    }
            //})
            //.WithoutBurst()
            //.Run();
            // Debug end
        }

        Dependency = JobHandle.CombineDependencies(jobHandles);

        jobHandles.Dispose();
    }

    public static bool ArrayBinarySearch(NativeArray<Entry> data, float item, out int indexInData)
    {
        int min = 0;
        int N = data.Length;
        int max = N - 1;
        do
        {            int mid = (min + max) / 2;            if (item > data[mid].position)
                min = mid + 1;            else
                max = mid - 1;            if (data[mid].position == item)
            {
                indexInData = mid;
                return true;
            }

        } while (min <= max);

        indexInData = max;
        return false;
    }

    public Entry GetCarInFront(float lane, float ownProgress)
    {
        if (m_CarInfoByLaneGroup == null)
        {
            return new Entry { position = float.MaxValue, speed = float.MaxValue };
        }

        int laneGroup = TrackGroup.GetTrackGroupIdx(lane);

        Entry entryWithMinProgressInFront = new Entry { position = float.MaxValue, speed = float.MaxValue };
        float minDistanceInFrontOfUs = float.MaxValue;

        GetEntryWithMinProgressInFront(laneGroup, ownProgress, ref minDistanceInFrontOfUs, ref entryWithMinProgressInFront);

        if (laneGroup > 0)
        {
            GetEntryWithMinProgressInFront(laneGroup - 1, ownProgress, ref minDistanceInFrontOfUs, ref entryWithMinProgressInFront);
        }
        if (laneGroup < numLaneGroup - 1)
        {
            GetEntryWithMinProgressInFront(laneGroup + 1, ownProgress, ref minDistanceInFrontOfUs, ref entryWithMinProgressInFront);
        }

        return entryWithMinProgressInFront;
    }

    static float GetDistanceInFront(float ownProgress, float frontProgress, float trackLength)
    {
        if (frontProgress >= ownProgress)
        {
            return frontProgress - ownProgress;
        }
        else
        {
            return (trackLength - ownProgress) + frontProgress;
        }
    }

    void GetEntryWithMinProgressInFront(int laneGroup, float ownProgress, ref float minDistanceInFront, ref Entry entryWithMinProgressInFront)
    {
        if (laneGroup < 0 || laneGroup > 6)
        {
            UnityEngine.Debug.Log("Oh no");
        }

        var carInfos = m_CarInfoByLaneGroup[laneGroup];
        if (carInfos.Length > 0)
        {
            var found = ArrayBinarySearch(carInfos, ownProgress, out var indexInData);
            if (!found || carInfos.Length > 1)
            {
                var entryInFront = carInfos[(indexInData + 1) % carInfos.Length];

                var distanceInFront = GetDistanceInFront(ownProgress, entryInFront.position, trackLength);

                if (distanceInFront < minDistanceInFront)
                {
                    entryWithMinProgressInFront = entryInFront;
                    minDistanceInFront = distanceInFront;
                }
            }           
        }
    }
}
