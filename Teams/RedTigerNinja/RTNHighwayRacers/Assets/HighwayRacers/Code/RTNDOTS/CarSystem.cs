using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

namespace HighwayRacers {

    public class CarSystem : JobComponentSystem
    {
        private EntityQuery CacheQueryCar, CacheQueryHighway;

        protected override JobHandle OnUpdate(JobHandle inputDeps)
        {
            if (Car.IsTotalHackData) return inputDeps;

            if (Highway.instance == null) return inputDeps;

            Highway.instance.EnsureUpdated();

            var carQuery = this.CacheQueryCar ?? this.GetEntityQuery(ComponentType.ReadOnly<CarLocation>());
            this.CacheQueryCar = carQuery;
            var carAr = carQuery.ToComponentDataArray<CarLocation>(Allocator.TempJob);
            var carSorted = new NativeArray<CarLocation>(carAr, Allocator.TempJob);

            var hpcQuery = CacheQueryHighway ?? this.GetEntityQuery(ComponentType.ReadOnly<HighwayPiece.HighwayPieceState>());
            this.CacheQueryHighway = hpcQuery;
            var hpcAr = hpcQuery.ToComponentDataArray<HighwayPiece.HighwayPieceState>(Allocator.TempJob);

            var highway = Highway.instance.HighwayState;
            highway.AllCars = carSorted;
            highway.AllPieces = hpcAr;
            highway.CarByEntityId = this.GetComponentDataFromEntity<CarLocation>(true);

            // testing code:
            var isTestSum = false;
            if (isTestSum)
            {
                NativeQueue<float> sq = new NativeQueue<float>(Allocator.TempJob);
                inputDeps = (new CarAverageSpeedJob() { Writer = sq.AsParallelWriter() }).Schedule(this, inputDeps);
                inputDeps.Complete();
                float sum = 0.0f;
                int count = 0;
                float current;
                while (sq.TryDequeue(out current))
                {
                    sum += current;
                    count++;
                }
                var avg = sum / ((float)count);
                Debug.Log("Average = " + avg);
            }

            // sort the cars:
            inputDeps = (new CarSortJob() { cars = carSorted }).Schedule(inputDeps);

            // calculate next state:
            inputDeps = (new CarUpdateNextJob(
                //this.GetComponentDataFromEntity<CarStateStruct>(true),
                highway,
                Car.CarUpdateInfo.Now()
                )).Schedule(this, inputDeps);
                
            // update current from next state:
            inputDeps = (new CarUpdateStateFromNextJob()).Schedule(this, inputDeps);

            // update the car position:
            inputDeps = (new CarUpdatePoseJob() { HighwayState = highway }).Schedule(this, inputDeps);

            //inputDeps.Complete();
            //hpcAr.Dispose();
            //carAr.Dispose();
            //carSorted.Dispose();

            inputDeps = (new DisposeJob<NativeArray<HighwayPiece.HighwayPieceState>>(hpcAr)).Schedule(inputDeps);
            inputDeps = (new DisposeJob<NativeArray<CarLocation>>(carAr)).Schedule(inputDeps);
            inputDeps = (new DisposeJob<NativeArray<CarLocation>>(carSorted)).Schedule(inputDeps);
            //inputDeps.Complete();

            //inputDeps.Complete();

            return inputDeps;
        }

        [BurstCompile]
        public struct CarAverageSpeedJob : IJobForEach<CarLocation>
        {
            public NativeQueue<float>.ParallelWriter Writer;

            public void Execute(ref CarLocation c0)
            {
                if (c0.IsInLane(2.0f)) {
                    this.Writer.Enqueue(c0.velocityPosition);
                }
            }
        }

        [BurstCompile]
        public struct CarSorter : System.Collections.Generic.IComparer<CarLocation>
        {
            public int Compare(CarLocation a, CarLocation b)
            {
                return a.laneRefDistance.CompareTo(b.laneRefDistance);
            }
        }

        [BurstCompile]
        public struct CarSortJob : IJob
        {
            public NativeArray<CarLocation> cars;

            public void Execute()
            {
                cars.Sort(new CarSorter());
            }
        }

        public struct SortingData
        {
            public int SelfIndex;
            public int NextIndex;
            public float laneRefDistance;
        }

        public struct DisposeJob<T> : IJob where T : struct, System.IDisposable
        {
            [DeallocateOnJobCompletion] public T DisposeMe;

            public DisposeJob(T _ar) 
            {
                this.DisposeMe = _ar;
            }

            public void Execute()
            {
                // the deallocate does the job!
            }
        }

        [BurstCompile]
        public struct CarNextState : IComponentData
        {
            public CarDataAll NextState;

            //public CarNextState() { }
            public CarNextState(CarDataAll initial) { this.NextState = initial; }
        }

        [BurstCompile]
        public struct CarUpdateStateFromNextJob : IJobForEach<CarLocation, CarMindState, CarNextState>
        {
            public void Execute([WriteOnly] ref CarLocation carLoc,[WriteOnly] ref CarMindState carMind, [ReadOnly] ref CarNextState c1)
            {
                carLoc = c1.NextState.Location;
                carMind = c1.NextState.Mind;
            }
        }

        [BurstCompile]
        public struct CarUpdatePoseJob : IJobForEach<CarLocation, CarSettingsStruct, Unity.Transforms.Rotation, Unity.Transforms.Translation, CarRenderData>
        {
            public Highway.HighwayStateStruct HighwayState;

            public void Execute([ReadOnly] ref CarLocation c0, [ReadOnly] ref CarSettingsStruct carSettings,[WriteOnly] ref Rotation c1,[WriteOnly] ref Translation c2, [WriteOnly] ref CarRenderData rd)
            {
                var pose = Car.GetCarPose(ref c0, ref HighwayState);
                c1.Value = pose.rotation;
                c2.Value = pose.position;
                rd = new CarRenderData()
                {
                    Matrix = Matrix4x4.TRS(pose.position, pose.rotation, Vector3.one),
                    Color = Car.UpdateColor(ref c0, ref carSettings),
                };
            }
        }

        [BurstCompile]
        public struct CarUpdateNextJob : IJobForEach<CarNextState,CarLocation,CarMindState,CarSettingsStruct>
        {
            //public ComponentDataFromEntity<CarStateStruct> OtherCarQuery;
            public Highway.HighwayStateStruct HighwayInst;
            public Car.CarUpdateInfo UpdateInfo;

            //public CarUpdateJob() { }
            public CarUpdateNextJob(
                //ComponentDataFromEntity<CarStateStruct> query,
                Highway.HighwayStateStruct _highwayInst,
                Car.CarUpdateInfo _updateInfo)
            {
                //this.OtherCarQuery = query;
                this.HighwayInst = _highwayInst;
                this.UpdateInfo = _updateInfo;
            }

            public void Execute([WriteOnly] ref CarNextState nextState, [ReadOnly]ref CarLocation carLoc, [ReadOnly] ref CarMindState carMind, [ReadOnly] ref CarSettingsStruct carSettings)
            {
                CarDataAll temp = new CarDataAll()
                {
                    Location = carLoc,
                    Mind = carMind,
                    Settings = carSettings,
                };

                // Do the actual update logic:
                Car.UpdateCarData(ref temp.Location, temp.Settings, ref temp.Mind, ref this.HighwayInst, this.UpdateInfo);

                // write it into the next state:
                nextState.NextState = temp;
            }
        }
    }

}
