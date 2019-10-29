using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;

namespace HighwayRacers {

    public class CarSystem : JobComponentSystem
    {
        protected override JobHandle OnUpdate(JobHandle inputDeps)
        {
            var query = this.GetEntityQuery(ComponentType.ReadOnly<CarStateStruct>());
            var ar = query.ToComponentDataArray<CarStateStruct>(Allocator.TempJob);

            // calculate next state:
            inputDeps = (new CarUpdateNextJob
                (this.GetComponentDataFromEntity<CarStateStruct>(true), ar))
                .Schedule(this, inputDeps);


            inputDeps.Complete();
            ar.Dispose();

            // update current from next state:
            inputDeps = (new CarUpdateStateFromNextJob()).Schedule(this, inputDeps);

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

        [BurstCompile]
        public struct CarUpdateNextJob : IJobForEach<CarNextState>
        {
            public readonly ComponentDataFromEntity<CarStateStruct> OtherCarQuery;
            public NativeArray<CarStateStruct> OtherCarArray;

            //public CarUpdateJob() { }
            public CarUpdateNextJob(ComponentDataFromEntity<CarStateStruct> query,
            NativeArray<CarStateStruct> _carArray)
            {
                this.OtherCarQuery = query;
                this.OtherCarArray = _carArray;
            }

            public void Execute(ref CarNextState nextState)
            {
                var c0 = OtherCarQuery[nextState.NextState.ThisCarEntity];

                CarStateStruct temp = c0;
                temp.DEBUG_JobTester++;

                // Do the actual update logic:
                Car.UpdateCarState_FromJob(ref temp);

                // write it into the next state:
                nextState.NextState = temp;
            }
        }
    }

}
