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
        protected override JobHandle OnUpdate(JobHandle inputDeps)
        {
            if (Car.IsTotalHackData) return inputDeps;

            Highway.instance.EnsureUpdated();

            var carQuery = this.GetEntityQuery(ComponentType.ReadOnly<CarLocation>());
            var carAr = carQuery.ToComponentDataArray<CarLocation>(Allocator.TempJob);

            var hpcQuery = this.GetEntityQuery(ComponentType.ReadOnly<HighwayPiece.HighwayPieceState>());
            var hpcAr = hpcQuery.ToComponentDataArray<HighwayPiece.HighwayPieceState>(Allocator.TempJob);

            var highway = Highway.instance.HighwayState;
            highway.AllCars = carAr;
            highway.AllPieces = hpcAr;

            // sort the cars:

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

            inputDeps.Complete();
            hpcAr.Dispose();
            carAr.Dispose();

            //inputDeps = (new DisposeJob<NativeArray<HighwayPiece.HighwayPieceState>>(hpcAr)).Schedule(inputDeps);
            //inputDeps = (new DisposeJob<NativeArray<CarStateStruct>>(carAr)).Schedule(inputDeps);
            //inputDeps.Complete();


            return inputDeps;
        }

        public struct SortingData
        {
            public int SelfIndex;
            public int NextIndex;
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


        public struct CarNextState : IComponentData
        {
            public CarDataAll NextState;

            //public CarNextState() { }
            public CarNextState(CarDataAll initial) { this.NextState = initial; }
        }

        public struct CarUpdateStateFromNextJob : IJobForEach<CarLocation, CarMindState, CarNextState>
        {
            public void Execute([WriteOnly] ref CarLocation carLoc,[WriteOnly] ref CarMindState carMind, [ReadOnly] ref CarNextState c1)
            {
                carLoc = c1.NextState.Location;
                carMind = c1.NextState.Mind;
            }
        }

        public struct CarUpdatePoseJob : IJobForEach<CarLocation, Unity.Transforms.Rotation, Unity.Transforms.Translation>
        {
            public Highway.HighwayStateStruct HighwayState;

            public void Execute([ReadOnly] ref CarLocation c0,[WriteOnly] ref Rotation c1,[WriteOnly] ref Translation c2)
            {
                var pose = Car.GetCarPose(ref c0, ref HighwayState);
                c1.Value = pose.rotation;
                c2.Value = pose.position;
            }
        }


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
                Car.UpdateCarState_FromJob(ref temp, ref this.HighwayInst, this.UpdateInfo);

                // write it into the next state:
                nextState.NextState = temp;
            }
        }
    }

}
