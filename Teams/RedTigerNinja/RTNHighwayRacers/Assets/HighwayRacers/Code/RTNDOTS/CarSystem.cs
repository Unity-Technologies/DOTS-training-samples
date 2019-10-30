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

            var carQuery = this.GetEntityQuery(ComponentType.ReadOnly<CarStateStruct>());
            var carAr = carQuery.ToComponentDataArray<CarStateStruct>(Allocator.TempJob);

            var hpcQuery = this.GetEntityQuery(ComponentType.ReadOnly<HighwayPiece.HighwayPieceState>());
            var hpcAr = hpcQuery.ToComponentDataArray<HighwayPiece.HighwayPieceState>(Allocator.TempJob);

            var highway = Highway.instance.HighwayState;
            highway.AllCars = carAr;
            highway.AllPieces = hpcAr;

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

            return inputDeps;
        }

        public struct CarNextState : IComponentData
        {
            public CarStateStruct NextState;

            //public CarNextState() { }
            public CarNextState(CarStateStruct initial) { this.NextState = initial; }
        }

        public struct CarUpdateStateFromNextJob : IJobForEach<CarStateStruct, CarNextState>
        {

            public void Execute(ref CarStateStruct c0, [ReadOnly] ref CarNextState c1)
            {
                c0 = c1.NextState;
            }
        }

        public struct CarUpdatePoseJob : IJobForEach<CarStateStruct, Unity.Transforms.Rotation, Unity.Transforms.Translation>
        {
            public Highway.HighwayStateStruct HighwayState;

            public void Execute([ReadOnly] ref CarStateStruct c0, ref Rotation c1, ref Translation c2)
            {
                var pose = Car.GetCarPose(ref c0, ref HighwayState);
                c1.Value = pose.rotation;
                c2.Value = pose.position;
            }
        }


        public struct CarUpdateNextJob : IJobForEach<CarNextState,CarStateStruct>
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

            public void Execute(ref CarNextState nextState, [ReadOnly] ref CarStateStruct curState)
            {

                var c0 = curState;// OtherCarQuery[nextState.NextState.ThisCarEntity];

                CarStateStruct temp = c0;
                temp.DEBUG_JobTester++;

                // Do the actual update logic:
                Car.UpdateCarState_FromJob(ref temp, ref this.HighwayInst, this.UpdateInfo);

                // write it into the next state:
                nextState.NextState = temp;
            }
        }
    }

}
