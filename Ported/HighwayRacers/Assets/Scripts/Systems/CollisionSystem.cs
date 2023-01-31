using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

[BurstCompile]
public partial struct CollisionSystem : ISystem
{
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
    }

    [BurstCompile]
    public void OnDestroy(ref SystemState state)
    {
    }
    
    
    public void OnUpdate(ref SystemState state)
    {
        var carEntities = SystemAPI.GetSingletonBuffer<CarEntity>().AsNativeArray();
        for (var i = 0; i < carEntities.Length; i++)
        {
            var entity = carEntities[i];
            entity.position = state.EntityManager.GetComponentData<CarPositionInLane>(entity.Value).Position;
        }

        var sortingJob = carEntities.SortJob(new CarEntityComparer());
        sortingJob.Schedule();
        
        //TODO Jobify
        for (var i = 0; i < carEntities.Length; i++)
        {
            var entity = carEntities[i];
            state.EntityManager.SetComponentData(entity.Value, new CarIndex{Index = i});
        }
        
        var collisionJob = new CollisionJob { CarEntities = carEntities };
        collisionJob.Schedule();
    }
}

[BurstCompile]
partial struct CollisionJob : IJobEntity
{
    public NativeArray<CarEntity> CarEntities;

    //TODO: Make this component?
    private const float CarSize = 1;
    
    [BurstCompile]
    void Execute(ref CarCollision collision, in CarIndex carIndex, in CarPositionInLane positionInLane)
    {
        //TODO: Define how many lanes and use this to determine how many cars to check
        for (int i = -4; i <= 4; i++)
        {
            if (i == 0) continue;
            int wrappedIndex = WrappedIndex(i);

            var otherCar = CarEntities[wrappedIndex];
            var otherCarPosition = SystemAPI.GetComponent<CarPositionInLane>(otherCar.Value);
            
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

    [BurstCompile]
    private int WrappedIndex(int index)
    {
        if (index < 0)
        {
            index += CarEntities.Length;
        }
        
        return index % CarEntities.Length;
    }
}