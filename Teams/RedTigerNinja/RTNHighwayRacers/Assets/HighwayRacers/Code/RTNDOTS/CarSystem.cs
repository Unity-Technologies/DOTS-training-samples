using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;

namespace HighwayRacers {

    public class CarSystem : JobComponentSystem
    {
        public void SpawnEntity(Car car)
        {
        }

        protected override JobHandle OnUpdate(JobHandle inputDeps)
        {
            // calculate next state:
            inputDeps = (new CarUpdateNextJob
                (this.GetComponentDataFromEntity<CarStateStruct>(true)))
                .Schedule(this, inputDeps);

            // update current from next state:
            inputDeps = (new CarUpdateStateFromNextJob()).Schedule(this, inputDeps);

            return inputDeps;
        }

        public struct CarNextState : IComponentData
        {
            public CarStateStruct NextState;
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

            //public CarUpdateJob() { }
            public CarUpdateNextJob(ComponentDataFromEntity<CarStateStruct> query)
            {
                this.OtherCarQuery = query;
            }

            public void Execute(ref CarNextState nextState)
            {
                var c0 = OtherCarQuery[nextState.NextState.ThisCarEntity];

                CarStateStruct temp = c0;
                Car.UpdateCarState_FromJob(ref temp);
                nextState.NextState = temp;


                //var otherCar = Unity.Entities.World.Active.EntityManager.GetComponentData<CarStateStruct>(c0.overtakeCarEntity);

                //c0.DEBUG_JobTester += otherCar.DEBUG_JobTester;
                c0.DEBUG_JobTester++;
            }
        }
    }

}
