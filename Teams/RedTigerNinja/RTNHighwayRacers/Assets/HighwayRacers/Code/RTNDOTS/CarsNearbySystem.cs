using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Jobs;
using Unity.Collections;
using Unity.Transforms;
using Unity.Mathematics;
using Unity.Burst;

namespace HighwayRacers
{
    public struct CarsNearbyData : IComponentData
    {
        public FixedSize<OtherPoint> Refs;
        public float3 MyPosition;

        public static int MAX_COUNT { get { return FixedSize<OtherPoint>.MAX_COUNT; } }
    }

    public struct OtherPoint
    {
        public float Distance;
        public float3 Position;
        public int CarId;
    }

    [BurstCompile]
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

        public bool InsertAt(int ndx, T orig)
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
            return TryAdd(data);

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
        public struct AngleSorter : System.Collections.Generic.IComparer<LocalToWorld>
        {
            public float3 WorldCenter;

            public float AngleValue(Vector3 v)
            {
                var p = (v - (Vector3)WorldCenter);
                return math.atan2(p.z, p.x);
            }

            public int Compare(LocalToWorld x, LocalToWorld y)
            {
                var a1 = AngleValue(x.Position);
                var a2 = AngleValue(y.Position);
                return a1.CompareTo(a2);
            }
        }

        public EntityQuery PoseQuery;
        public EntityQuery RenderQuery;

        protected override JobHandle OnUpdate(JobHandle inputDeps)
        {
            var isSkipThis = true;
            if (isSkipThis)
            {
                return inputDeps;
            }

            inputDeps = (new NearbyCarsReset()).Schedule(this, inputDeps);

            //return inputDeps;

            var em = this.EntityManager;

            var posQuery = PoseQuery ?? em.CreateEntityQuery(ComponentType.ReadOnly<LocalToWorld>());
            PoseQuery = posQuery;
            var poses = posQuery.ToComponentDataArray<LocalToWorld>(Allocator.TempJob);

            var sorted = new NativeArray<LocalToWorld>(poses, Allocator.TempJob);
            var sorter = new AngleSorter() { WorldCenter = CarRenderSystem.instance.ReferenceCenter.position };

            inputDeps = (new NearbyCarsSort() { SourceArray = poses, DestArray = sorted, Sorter = sorter }).Schedule(inputDeps);

            inputDeps = (new NearbyCarsJob() { AllCars = sorted, Angler =  sorter}).Schedule(this, inputDeps);

            //inputDeps.Complete();


            //return inputDeps;

            inputDeps.Complete();

            var debugData = RenderQuery ?? em.CreateEntityQuery(ComponentType.ReadOnly<CarsNearbyData>());
            this.RenderQuery = debugData;
            var debugArray = debugData.ToComponentDataArray<CarsNearbyData>(Allocator.TempJob);
            inputDeps = CarRenderJobSystem.DrawNearbyStuff(inputDeps, debugArray);
            debugArray.Dispose();

            // Cleanup:
            poses.Dispose();
            sorted.Dispose();
            //inputDeps = (new CarSystem.DisposeJob<NativeArray<LocalToWorld>>(poses)).Schedule(inputDeps);
            //inputDeps = (new CarSystem.DisposeJob<NativeArray<LocalToWorld>>(sorted)).Schedule(inputDeps);

            return inputDeps;
        }

        [BurstCompile]
        public struct NearbyCarsSort : IJob
        {
            [ReadOnly] public NativeArray<LocalToWorld> SourceArray;
            public NativeArray<LocalToWorld> DestArray;
            public AngleSorter Sorter;

            public void Execute()
            {
                DestArray.CopyFrom(this.SourceArray);
                DestArray.Sort(this.Sorter);
            }
        }


        [BurstCompile]
        public struct NearbyCarsReset : IJobForEach<CarsNearbyData>
        {
            public void Execute(ref CarsNearbyData c0)
            {
                var lst = c0.Refs;
                lst.Clear();
                c0.Refs = lst;
            }
        }

        [BurstCompile]
        public struct NearbyCarsJob : IJobForEach<CarsNearbyData, LocalToWorld>
        {
            [ReadOnly] public NativeArray<LocalToWorld> AllCars;
            public AngleSorter Angler;

            public IntRange GetRange(Vector3 pos, int maxIndexOffset)
            {
                IntRange range = new IntRange(0, AllCars.Length);
                var goalAngle = Angler.AngleValue(pos);

                while (range.Min+2 < range.Max)
                {
                    var mid = (range.Min + range.Max) / 2;
                    var entry = this.AllCars[mid];
                    var ang = Angler.AngleValue(entry.Position);
                    if (goalAngle < ang)
                    {
                        range.Max = mid;
                    }
                    else
                    {
                        range.Min = mid;
                    }
                }

                range.Min = math.max(0, (range.Min - maxIndexOffset));
                range.Max = math.min(AllCars.Length - 1, range.Max + maxIndexOffset);

                return range;

            }

            public struct IntRange
            {
                public int Min, Max;
                public IntRange(int mn, int mx) { Min = mn; Max = mx; }
            }

            public void Execute(ref CarsNearbyData c0,[ReadOnly] ref LocalToWorld c1)
            {
                var lst = c0.Refs;
                var rang = this.GetRange(c1.Position, 15);
                for (var i= rang.Min; i<rang.Max; i++)
                {
                    var other = AllCars[i];
                    var delta = (Vector3)(c1.Position - other.Position);
                    var dist = delta.magnitude;
                    if ((dist < 20.0f) && (dist > 0.1f))
                    {
                        int bestIndex = 0;
                        while (bestIndex < lst.Count)
                        {
                            if (lst.Get(bestIndex).Distance < dist)
                            {
                                // current is better
                                bestIndex++;
                            }
                            else
                            {
                                break;
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


}