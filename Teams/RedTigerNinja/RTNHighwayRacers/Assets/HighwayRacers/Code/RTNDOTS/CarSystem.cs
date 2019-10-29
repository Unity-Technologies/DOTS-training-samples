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
            inputDeps = (new CarUpdateJob() { }).Schedule(this, inputDeps);
            return inputDeps;
        }

        [BurstCompile]
        public struct CarUpdateJob : IJobForEach<CarStateStruct>
        {
            public void Execute(ref CarStateStruct c0)
            {

                //var otherCar = Unity.Entities.World.Active.EntityManager.GetComponentData<CarStateStruct>(c0.overtakeCarEntity);

                //c0.DEBUG_JobTester += otherCar.DEBUG_JobTester;
                c0.DEBUG_JobTester++;
            }
        }
    }

}
