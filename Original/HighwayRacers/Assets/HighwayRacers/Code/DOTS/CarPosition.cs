using System;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;

[Serializable]
public struct CarPosition : IComponentData
{
    public float Distance;
    public float Lane;
}

[Serializable]
public unsafe struct CarPositionStaticProperties : IComponentData
{
    public fixed float MergeSpace[4];
}

public class CarPositionSystem : JobComponentSystem
{
    public NativeArray<Entity> SortedCars;

    private EntityQuery m_AllCars;

    private struct SortCarPositionJob : IJob
    {
        public NativeArray<CarPosition> CarPositions;
        public NativeList<CarPosition> Lane1;
        public NativeList<CarPosition> Lane2;
        public NativeList<CarPosition> Lane3;
        public NativeList<CarPosition> Lane4;

        public void Execute()
        {
        }
    }

    protected override void OnCreate()
    {
        m_AllCars = GetEntityQuery(typeof(CarPosition));
    }

    protected override JobHandle OnUpdate(JobHandle inputDeps)
    {
        var allCars = m_AllCars.ToComponentDataArray<CarPosition>(Allocator.TempJob);

        return new SortCarPositionJob()
        {
            CarPositions = allCars,
            Lane1 = new NativeList<CarPosition>(Allocator.TempJob),
            Lane2 = new NativeList<CarPosition>(Allocator.TempJob),
            Lane3 = new NativeList<CarPosition>(Allocator.TempJob),
            Lane4 = new NativeList<CarPosition>(Allocator.TempJob),
        }.Schedule(inputDeps);
    }
}
