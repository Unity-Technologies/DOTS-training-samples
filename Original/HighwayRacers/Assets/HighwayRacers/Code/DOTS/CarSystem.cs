using System;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;

public class CarSystem : JobComponentSystem
{
    private struct EntityAndPosition
    {
        public Entity Entity;
        public CarPosition Position;
    }

    // Sort cars into a list of each lane. Cars might be in two lanes if it's changing lanes.
    private struct LaneSortJob : IJobForEachWithEntity<CarLane, CarPosition>
    {
        public NativeList<EntityAndPosition> Lane0;
        public NativeList<EntityAndPosition> Lane1;
        public NativeList<EntityAndPosition> Lane2;
        public NativeList<EntityAndPosition> Lane3;

        private static ref NativeList<EntityAndPosition> GetLaneList(ref LaneSortJob job, int index)
        {
            if (index == 0)
                return ref job.Lane0;
            else if (index == 1)
                return ref job.Lane1;
            else if (index == 2)
                return ref job.Lane2;
            else
                return ref job.Lane3;
        }

        public void Execute(Entity entity, int index, [ReadOnly] ref CarLane carLane, [ReadOnly] ref CarPosition carPosition)
        {
            int lane1 = (int)math.floor(carLane.Lane);
            int lane2 = (int)math.ceil(carLane.Lane);
            GetLaneList(ref this, lane1).Add(new EntityAndPosition() { Entity = entity, Position = carPosition });
            if (lane2 != lane1)
                GetLaneList(ref this, lane2).Add(new EntityAndPosition() { Entity = entity, Position = carPosition });
        }
    }

    private struct PositionSortJob : IJobParallelFor
    {
        public NativeArray<EntityAndPosition> Lane0;
        public NativeArray<EntityAndPosition> Lane1;
        public NativeArray<EntityAndPosition> Lane2;
        public NativeArray<EntityAndPosition> Lane3;

        private static ref NativeArray<EntityAndPosition> GetLaneArray(ref PositionSortJob job, int index)
        {
            if (index == 0)
                return ref job.Lane0;
            else if (index == 1)
                return ref job.Lane1;
            else if (index == 2)
                return ref job.Lane2;
            else
                return ref job.Lane3;
        }

        private struct PositionSort : IComparer<EntityAndPosition>
        {
            public int Compare(EntityAndPosition x, EntityAndPosition y)
                => x.Position.Position.CompareTo(y.Position.Position);
        }

        public void Execute(int index)
        {
            ref var laneEntities = ref GetLaneArray(ref this, index);
            laneEntities.Sort(new PositionSort());
        }
    }

    protected override JobHandle OnUpdate(JobHandle inputDeps)
    {
        var carEntitiesLane0 = new NativeList<EntityAndPosition>(Allocator.TempJob);
        var carEntitiesLane1 = new NativeList<EntityAndPosition>(Allocator.TempJob);
        var carEntitiesLane2 = new NativeList<EntityAndPosition>(Allocator.TempJob);
        var carEntitiesLane3 = new NativeList<EntityAndPosition>(Allocator.TempJob);

        inputDeps = new LaneSortJob()
        {
            Lane0 = carEntitiesLane0,
            Lane1 = carEntitiesLane1,
            Lane2 = carEntitiesLane2,
            Lane3 = carEntitiesLane3,
        }.Schedule(this, inputDeps);

        inputDeps = new PositionSortJob()
        {
            Lane0 = carEntitiesLane0.AsDeferredJobArray(),
            Lane1 = carEntitiesLane1.AsDeferredJobArray(),
            Lane2 = carEntitiesLane2.AsDeferredJobArray(),
            Lane3 = carEntitiesLane3.AsDeferredJobArray(),
        }.Schedule(4, 1, inputDeps);

        // 1. Put cars {Entity, CarPosition} into different lanes
        // 2. Sort the list of cars for each lane => {Entity, index}
        // 3. Car logic {Entity, index, velocity, state }
        // 4. Compose the matrices for each mesh instance for correct rendering.
        return inputDeps;
    }
}
