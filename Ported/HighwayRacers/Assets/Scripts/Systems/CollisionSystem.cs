using System;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Rendering;
using Random = UnityEngine.Random;

[assembly: RegisterGenericJobType(typeof(SortJob<CarEntity, CarEntityComparer>))] 
[BurstCompile]
//[UpdateAfter(typeof(CarSpawnerSystem))]
public partial struct CollisionSystem : ISystem
{
    
    private ComponentLookup<CarPositionInLane> carPositionInLaneLookup;
    private ComponentLookup<CarVelocity> carVelocityLookup;
    private ComponentLookup<CarIndex> carIndexLookup;
    private ComponentLookup<URPMaterialPropertyBaseColor> colorLookup;

    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<CarEntity>();
        carPositionInLaneLookup = SystemAPI.GetComponentLookup<CarPositionInLane>();
        carVelocityLookup = SystemAPI.GetComponentLookup<CarVelocity>();
        carIndexLookup = SystemAPI.GetComponentLookup<CarIndex>();
        colorLookup = SystemAPI.GetComponentLookup<URPMaterialPropertyBaseColor>();
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
        carVelocityLookup.Update(ref state);
        colorLookup.Update(ref state);
        
        var carEntities = SystemAPI.GetSingletonBuffer<CarEntity>().AsNativeArray();
        int entitiesLength = carEntities.Length;
        int batchSize = 100;
            
        var positionAssignmentJob = new PositionAssignmentJob
        {
            CarEntities = carEntities, CarPositionInLaneLookup = carPositionInLaneLookup, ColorLookup = colorLookup, random = new Unity.Mathematics.Random((uint)Random.Range(1,int.MaxValue))
        };
        state.Dependency = positionAssignmentJob.Schedule(entitiesLength, batchSize,state.Dependency);
            
        var sortingJob = carEntities.SortJob(new CarEntityComparer());
        state.Dependency = sortingJob.Schedule(state.Dependency);

        var indexAssignmentJob = new IndexAssignmentJob { CarEntities = carEntities, CarIndexLookup = carIndexLookup, ColorLookup = colorLookup};
        state.Dependency = indexAssignmentJob.Schedule(entitiesLength, batchSize, state.Dependency);
        
        var collisionJob = new CollisionJob { CarEntities = carEntities, CarPositionInLaneLookup = carPositionInLaneLookup, CarVelocityLookup = carVelocityLookup};
        state.Dependency = collisionJob.Schedule(state.Dependency);
    }
}

partial struct PositionAssignmentJob : IJobParallelFor
{
    public NativeArray<CarEntity> CarEntities;
    [ReadOnly] public ComponentLookup<CarPositionInLane> CarPositionInLaneLookup;
    [NativeDisableParallelForRestriction] public ComponentLookup<URPMaterialPropertyBaseColor> ColorLookup;
    public Unity.Mathematics.Random random;

    public void Execute(int index)
    {
        var entity = CarEntities[index];
        entity.position = CarPositionInLaneLookup[entity.Value].Position;
        CarEntities[index] = entity;    
        // ColorLookup[CarEntities[index].Value] = 
        //     new URPMaterialPropertyBaseColor
        //     {
        //         Value = new float4 (random.NextFloat() % 1.0f,0,0,0)
        //     };
        
    }
}

partial struct IndexAssignmentJob : IJobParallelFor
{
    [ReadOnly] public NativeArray<CarEntity> CarEntities;
    [NativeDisableParallelForRestriction] public ComponentLookup<CarIndex> CarIndexLookup;
    [NativeDisableParallelForRestriction] public ComponentLookup<URPMaterialPropertyBaseColor> ColorLookup;
    

    public void Execute(int index)
    {
        CarIndexLookup[CarEntities[index].Value] = new CarIndex{Index = index};
            
        // ColorLookup[CarEntities[index].Value] = 
        //     new URPMaterialPropertyBaseColor
        //     {
        //         Value = new float4 ((float)index / (float)CarEntities.Length,0,0,0)
        //     };
    }
}

[BurstCompile]
partial struct CollisionJob : IJobEntity
{
    public NativeArray<CarEntity> CarEntities;
    [ReadOnly] public ComponentLookup<CarPositionInLane> CarPositionInLaneLookup;
    [ReadOnly] public ComponentLookup<CarVelocity> CarVelocityLookup;

    //TODO: Make this component?
    private const float CarRadius = 10;
    
    void Execute(ref CarCollision collision, in CarIndex carIndex, in CarPositionInLane positionInLane, ref URPMaterialPropertyBaseColor baseColor)
    {

        collision.Front = false;
        collision.Left = false;
        collision.Right = false;
        collision.FrontVelocity = 0.0f;
        collision.FrontDistance = 0.0f;
        
        //TODO: Define how many lanes and use this to determine how many cars to check
        for (int i = -3; i <= 7; i++)
        {
            if (i == 0) continue;
            int wrappedIndex = WrappedIndex(i + carIndex.Index);

            var otherCar = CarEntities[wrappedIndex];
            var otherCarPosition = CarPositionInLaneLookup[otherCar.Value];
            
            //Front:
            bool sameLane = math.abs(otherCarPosition.Lane - positionInLane.Lane) < 0.8f;

            if (sameLane)
            {
                float distance = otherCarPosition.Position - positionInLane.Position;
                if (distance > 0 && distance < CarRadius)
                {
                    collision.Front = true;
                    collision.FrontVelocity = CarVelocityLookup[otherCar.Value].VelY;
                    collision.FrontDistance = CarRadius - distance; //calc distance to perfect following distance
                }
            }
        }
        
        
        //DEBUG
        if (collision.Front)
        {
            baseColor.Value = new float4(1.0f,1.0f,1.0f,1.0f);
        }
        else
        {
            baseColor.Value = new float4(0,0,0,0);
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