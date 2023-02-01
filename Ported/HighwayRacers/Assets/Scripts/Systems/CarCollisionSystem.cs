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
public partial struct CarCollisionSystem : ISystem
{
    [ReadOnly]private ComponentLookup<CarPositionInLane> carPositionInLaneLookup;
    [ReadOnly]private ComponentLookup<CarVelocity> carVelocityLookup;
    private ComponentLookup<CarIndex> carIndexLookup;
    private ComponentLookup<URPMaterialPropertyBaseColor> colorLookup;

    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<CarEntity>();
        carPositionInLaneLookup = SystemAPI.GetComponentLookup<CarPositionInLane>(true);
        carVelocityLookup = SystemAPI.GetComponentLookup<CarVelocity>(true);
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

        var spawner = SystemAPI.GetSingleton<GlobalSettings>();
        
        var positionAssignmentJob = new PositionAssignmentJob
        {
            CarEntities = carEntities, CarPositionInLaneLookup = carPositionInLaneLookup, ColorLookup = colorLookup,
            random = new Unity.Mathematics.Random((uint)Random.Range(1, int.MaxValue))
        };
        state.Dependency = positionAssignmentJob.Schedule(entitiesLength, batchSize, state.Dependency);

        var sortingJob = carEntities.SortJob(new CarEntityComparer());
        state.Dependency = sortingJob.Schedule(state.Dependency);

        var indexAssignmentJob = new IndexAssignmentJob
            { CarEntities = carEntities, CarIndexLookup = carIndexLookup, ColorLookup = colorLookup };
        state.Dependency = indexAssignmentJob.Schedule(entitiesLength, batchSize, state.Dependency);

        var collisionJob = new CollisionJob
        {
            CarEntities = carEntities, CarPositionInLaneLookup = carPositionInLaneLookup,
            CarVelocityLookup = carVelocityLookup,
            MaxLaneIndex = spawner.NumLanes - 1,
            LaneLength = spawner.LengthLanes
        };
        collisionJob.ScheduleParallel();
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
        CarIndexLookup[CarEntities[index].Value] = new CarIndex { Index = index };

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
    public int MaxLaneIndex;
    public float LaneLength;
    [ReadOnly] public NativeArray<CarEntity> CarEntities;
    [ReadOnly] public ComponentLookup<CarPositionInLane> CarPositionInLaneLookup;
    [ReadOnly] public ComponentLookup<CarVelocity> CarVelocityLookup;

    //TODO: Make this component?
    private const float CarRadius = 2;

    [BurstCompile]
    void Execute(ref CarCollision collision, in CarIndex carIndex, in CarPositionInLane positionInLane,
        ref URPMaterialPropertyBaseColor baseColor)
    {
        collision.Front = false;
        collision.Left = positionInLane.LaneIndex == 0;
        collision.Right = positionInLane.LaneIndex == MaxLaneIndex;
        collision.FrontVelocity = 0.0f;
        collision.FrontDistance = 0.0f;


        int startIndex = -(MaxLaneIndex - 1);
        int endIndex = 1 + 2 * (MaxLaneIndex - 1);
        //TODO: Define how many lanes and use this to determine how many cars to check
        for (int i = startIndex; i <= endIndex; i++)
        {
            if (i == 0) continue;
            int wrappedIndex = WrappedIndex(i + carIndex.Index, out int wrapDirection);

            var otherCar = CarEntities[wrappedIndex];
            var otherCarPosition = CarPositionInLaneLookup[otherCar.Value];

            int otherCarLane = otherCarPosition.LaneIndex;
            int lane = positionInLane.LaneIndex;
            
            //Front:
            bool sameLane = otherCarLane == lane;

            float distance = (otherCarPosition.Position + LaneLength * wrapDirection) - positionInLane.Position;
            float absoluteDistance = math.abs(distance);

            if (sameLane && distance > 0 && distance < CarRadius)
            {
                collision.Front = true;
                collision.FrontVelocity = CarVelocityLookup[otherCar.Value].VelY;
                collision.FrontDistance = CarRadius - distance; //calc distance to perfect following distance
            }
            else if (absoluteDistance > CarRadius)
            {
                continue;
            }

            if (otherCarLane == lane + 1)
            {
                collision.Right = true;
            }
            else if (otherCarLane == lane - 1)
            {
                collision.Left = true;
            }
        }


        //DEBUG
        if (collision.Front)
        {
            baseColor.Value = new float4(1.0f, 1.0f, 1.0f, 1.0f);
        }
        else
        {
            baseColor.Value = new float4(0, 0, 0, 0);
        }
    }

   [BurstCompile] 
    private int WrappedIndex(int index, out int wrapDirection)
    {
        wrapDirection = 0;
        
        if (index < 0)
        {
            wrapDirection = -1;
            index += CarEntities.Length;
        }

        if (index >= CarEntities.Length)
        {
            wrapDirection = 1;
            index %= CarEntities.Length;
        }

        return index;
    }
}