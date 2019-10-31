using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Jobs;
using Unity.Collections;
using Unity.Transforms;
using Unity.Mathematics;

namespace HighwayRacers
{
    public struct CarsNearbyData : IComponentData
    {
        public Vector3 MyPosition;
    }

    /*
    public struct CarsNearbyData : IComponentData
    {
        public FixedSize<OtherPoint> Refs;
        public Vector3 MyPosition;
    }

    public struct OtherPoint
    {
        public float Distance;
        public Vector3 Position;
        public int CarId;
    }

    public struct FixedSize<T>
    {
        public T Ref0, Ref1, Ref2, Ref3;
        public int Count;
        public const int MAX_COUNT = 4;

        public int MaxCount { get { return MAX_COUNT; } }

        public void Clear()
        {
            Clear(ref this);
        }

        public bool TryAdd(T data)
        {
            return TryAdd(ref this, data);
        }

        public void InsertAt(int ndx, T orig)
        {
            var to = ndx;
            var data = orig;
            while (to < Count)
            {
                var swp = Get(to);
                Set(ref this, to, data);

                data = swp;
                to++;
            }
            TryAdd(data);

        }

        public static void Clear(ref FixedSize<T> into)
        {
            into.Count = 0;
        }

        public static bool TryAdd(ref FixedSize<T> into, T data)
        {
            if (into.Count < MAX_COUNT)
            {
                Set(ref into, into.Count, data);
                into.Count++;
                return true;
            }
            return false;
        }

        public T Get(int i)
        {
            switch (i % MAX_COUNT)
            {
                case 0: return this.Ref0;
                case 1: return this.Ref1;
                case 2: return this.Ref2;
                case 3: return this.Ref3;
                default: return default(T);
            }
        }

        public static void Set(ref FixedSize<T> into, int i, T val) { 
            switch (i % MAX_COUNT)
            {
                case 0: into.Ref0 = val; break;
                case 1: into.Ref1 = val; break;
                case 2: into.Ref2 = val; break;
                case 3: into.Ref3 = val; break;
                default: break;
            }
        }

    }

    public class CarsNearbySystem : JobComponentSystem
    {
        protected override JobHandle OnUpdate(JobHandle inputDeps)
        {
            var em = this.EntityManager;

            var posQuery = em.CreateEntityQuery(ComponentType.ReadOnly<LocalToWorld>());
            var poses = posQuery.ToComponentDataArray<LocalToWorld>(Allocator.TempJob);

            inputDeps = (new NearbyCarsReset()).Schedule(this, inputDeps);

            inputDeps = (new NearbyCarsJob() { AllCars = poses }).Schedule(this, inputDeps);

            inputDeps.Complete();
            poses.Dispose();

            var debugData = em.CreateEntityQuery(ComponentType.ReadOnly<CarsNearbyData>());
            var debugArray = debugData.ToComponentDataArray<CarsNearbyData>(Allocator.TempJob);

            for (var i=0; i<debugArray.Length; i++)
            {
                var enty = debugArray[i];
                for (var j=0; j<enty.Refs.Count; j++)
                {
                    var to = enty.Refs.Get(j);
                    Debug.DrawLine(enty.MyPosition, to.Position);
                }
            }

            debugArray.Dispose();

            return inputDeps;
        }

        public struct NearbyCarsReset : IJobForEach<CarsNearbyData>
        {
            public void Execute(ref CarsNearbyData c0)
            {
                c0.Refs.Clear();
            }
        }

        public struct NearbyCarsJob : IJobForEach<CarsNearbyData, LocalToWorld>
        {
            [ReadOnly] public NativeArray<LocalToWorld> AllCars;

            public void Execute(ref CarsNearbyData c0,[ReadOnly] ref LocalToWorld c1)
            {
                var lst = c0.Refs;
                for (var i=0; i<AllCars.Length; i++)
                {
                    var other = AllCars[i];
                    var delta = (Vector3)(c1.Position - other.Position);
                    var dist = delta.magnitude;
                    if (dist < 20.0f)
                    {
                        int bestIndex = 0;
                        while (bestIndex < lst.Count)
                        {
                            if (lst.Get(bestIndex).Distance < dist)
                            {
                                // current is better
                                bestIndex++;
                            }
                        }
                        if (bestIndex < lst.MaxCount)
                        {
                            var entry = new OtherPoint() { Position = other.Position, Distance = dist, CarId = i };
                            lst.InsertAt(bestIndex, entry);
                        }
                    }
                }
                c0.MyPosition = c1.Position;
                c0.Refs = lst;
            }
        }
    }
    */

}