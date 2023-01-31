using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;

[assembly: RegisterGenericJobType(typeof(SortJob<CarEntity, CarEntityComparer>))] 
[BurstCompile]
[UpdateAfter(typeof(CarSpawnerSystem))]
public partial struct CollisionSystem : ISystem
{
    
    private ComponentLookup<CarPositionInLane> carPositionInLaneLookup;
    private ComponentLookup<CarIndex> carIndexLookup;

    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<CarEntity>();
        carPositionInLaneLookup = SystemAPI.GetComponentLookup<CarPositionInLane>();
        carIndexLookup = SystemAPI.GetComponentLookup<CarIndex>();
    }

    [BurstCompile]
    public void OnDestroy(ref SystemState state)
    {
    }
    
    //[BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        carIndexLookup.Update(ref state);
        carPositionInLaneLookup.Update(ref state);
        
        var carEntities = SystemAPI.GetSingletonBuffer<CarEntity>().AsNativeArray();
        int entitiesLength = carEntities.Length;
        int batchSize = 100;
            
        var positionAssignmentJob = new PositionAssignmentJob{ CarEntities = carEntities, CarPositionInLaneLookup = carPositionInLaneLookup};
        state.Dependency = positionAssignmentJob.Schedule(entitiesLength, batchSize,state.Dependency);
            
        var sortingJob = carEntities.SortJob(new CarEntityComparer());
        state.Dependency = sortingJob.Schedule(state.Dependency);

        var indexAssignmentJob = new IndexAssignmentJob { CarEntities = carEntities, CarIndexLookup = carIndexLookup };
        state.Dependency = indexAssignmentJob.Schedule(entitiesLength, batchSize, state.Dependency);
        
        var collisionJob = new CollisionJob { CarEntities = carEntities, CarPositionInLaneLookup = carPositionInLaneLookup};
        state.Dependency = collisionJob.Schedule(state.Dependency);
    }
}

partial struct PositionAssignmentJob : IJobParallelFor
{
    public NativeArray<CarEntity> CarEntities;
    [ReadOnly] public ComponentLookup<CarPositionInLane> CarPositionInLaneLookup;

    public void Execute(int index)
    {
        var entity = CarEntities[index];
        entity.position = CarPositionInLaneLookup[entity.Value].Position;
        CarEntities[index] = entity;    
    }
}

partial struct IndexAssignmentJob : IJobParallelFor
{
    [ReadOnly] public NativeArray<CarEntity> CarEntities;
    [NativeDisableParallelForRestriction] public ComponentLookup<CarIndex> CarIndexLookup;

    public void Execute(int index)
    {
        CarIndexLookup[CarEntities[index].Value] = new CarIndex{Index = index};
    }
}

[BurstCompile]
partial struct CollisionJob : IJobEntity
{
    public NativeArray<CarEntity> CarEntities;
    [ReadOnly] public ComponentLookup<CarPositionInLane> CarPositionInLaneLookup;

    //TODO: Make this component?
    private const float CarSize = 1;
    
    void Execute(ref CarCollision collision, in CarIndex carIndex, in CarPositionInLane positionInLane)
    {
        //TODO: Define how many lanes and use this to determine how many cars to check
        for (int i = -4; i <= 4; i++)
        {
            if (i == 0) continue;
            int wrappedIndex = WrappedIndex(i);

            var otherCar = CarEntities[wrappedIndex];
            var otherCarPosition = CarPositionInLaneLookup[otherCar.Value];
            
            //Front:
            bool sameLane = math.abs(otherCarPosition.Lane - positionInLane.Lane) < 1;

            if (sameLane)
            {
                float distance = otherCarPosition.Position - positionInLane.Position;
                if (distance > 0 && distance < CarSize)
                {
                    collision.Front = true;
                }
            }
        }
    }
    
    private int WrappedIndex(int index)
    {
        if (index < 0)
        {
            index += CarEntities.Length;
        }
        
        return index % CarEntities.Length;
    }
}