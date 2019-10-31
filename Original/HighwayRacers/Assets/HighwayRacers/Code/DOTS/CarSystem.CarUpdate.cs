using Unity.Burst;
using Unity.Collections;
using Unity.Entities;

partial class CarSystem
{
    [BurstCompile]
    private struct CarUpdateJob : IJobForEachWithEntity<CarBasicState>
    {
        public float Dt;
        public float HighwayLen;

        [ReadOnly] public CarQueryStructure QueryStructure;

        public void Execute(Entity entity, int index, [ReadOnly] ref CarBasicState carBasicState)
        {
            bool hasCarInFront = QueryStructure.GetCarInFront(index, carBasicState.Lane, HighwayLen, out var carInFront);
        }
    }
}
