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

    private bool _hasInitiallySorted;

    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<CarEntity>();
        carPositionInLaneLookup = SystemAPI.GetComponentLookup<CarPositionInLane>(true);
        carVelocityLookup = SystemAPI.GetComponentLookup<CarVelocity>(true);
        carIndexLookup = SystemAPI.GetComponentLookup<CarIndex>();
        colorLookup = SystemAPI.GetComponentLookup<URPMaterialPropertyBaseColor>();
        _hasInitiallySorted = false;
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

        //Initially sort with SortJob (Mergesort)
        if (!_hasInitiallySorted)
        {
            _hasInitiallySorted = true;
            var sortingJob = carEntities.SortJob(new CarEntityComparer());
            state.Dependency = sortingJob.Schedule(state.Dependency);
        }
        else //then sort with incremental Insertion Sort (~O(n))
        {
            var sortingJob = new InsertionSortJob { CarEntities = carEntities, Comparer = new CarEntityComparer(), };
            state.Dependency = sortingJob.Schedule(entitiesLength, state.Dependency);
        }

        var indexAssignmentJob = new IndexAssignmentJob
            { CarEntities = carEntities, CarIndexLookup = carIndexLookup, ColorLookup = colorLookup };
        state.Dependency = indexAssignmentJob.Schedule(entitiesLength, batchSize, state.Dependency);

        var collisionJob = new CollisionJob
        {
            CarEntities = carEntities,
            MaxLaneIndex = spawner.NumLanes - 1,
            LaneLength = spawner.LengthLanes
        };
        collisionJob.ScheduleParallel();
    }
}

[BurstCompile]
partial struct PositionAssignmentJob : IJobParallelFor
{
    public NativeArray<CarEntity> CarEntities;
    [ReadOnly] public ComponentLookup<CarPositionInLane> CarPositionInLaneLookup;
    [NativeDisableParallelForRestriction] public ComponentLookup<URPMaterialPropertyBaseColor> ColorLookup;
    public Unity.Mathematics.Random random;

    public void Execute(int index)
    {
        var entity = CarEntities[index];
        var positionInLane = CarPositionInLaneLookup[entity.Value];
        entity.Position = positionInLane.Position;
        entity.Lane = positionInLane.LaneIndex;
        CarEntities[index] = entity;
        // ColorLookup[CarEntities[index].Value] = 
        //     new URPMaterialPropertyBaseColor
        //     {
        //         Value = new float4 (random.NextFloat() % 1.0f,0,0,0)
        //     };
    }
}


/**
 * TODO: Insertion sort ST
 * TODO: later: Odd-Even (parallel) sort
 */

[BurstCompile]
partial struct InsertionSortJob : IJobFor
{
    public NativeArray<CarEntity> CarEntities;
    public CarEntityComparer Comparer;
    public void Execute(int index)
    {
        if (index == 0) return;
        //index determines sorted partition
        int reverseIndex = index - 1;
        
        //search backwards through array, stop when current index is greater or equal
        while (index > 0 && Comparer.Compare(CarEntities[index], CarEntities[reverseIndex]) < 0)
        {
            //swapping as we go
            SwapCarEntites(index--, reverseIndex--);
        }
        
    }

    public void SwapCarEntites(int a, int b)
    {
        CarEntity temp = CarEntities[a];
        CarEntities[a] = CarEntities[b];
        CarEntities[b] = temp;
    }
}

[BurstCompile]
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

    //TODO: Make this component?
    private const float CarRadius = 2;

    [BurstCompile]
    void Execute(ref CarCollision collision, in CarIndex carIndex, in CarPositionInLane positionInLane,
        ref URPMaterialPropertyBaseColor baseColor)
    {
        //TODO reduce amount of unnecessary data in structs
        
        //TODO refactor collision to use bitwise flags
        collision.FrontVelocity = 0.0f;
        collision.FrontDistance = 0.0f;
        
        collision.CollisionFlags = CollisionType.None;
        collision.CollisionFlags |= positionInLane.LaneIndex == 0  ? CollisionType.Left  : CollisionType.None;
        collision.CollisionFlags |= positionInLane.LaneIndex == MaxLaneIndex ? CollisionType.Right : CollisionType.None;
        


        int startIndex = -(MaxLaneIndex - 1);
        int endIndex = 1 + 2 * (MaxLaneIndex - 1);
        
        /*
         * TODO Ordering revision
         * [end:1] then
         * [-1:start]
         */
        for (int i = endIndex; i >= startIndex; i--)
        {
            //full out if found colls on all sides
            if (collision.CollisionFlags == (CollisionType.Left | CollisionType.Right | CollisionType.Front)) break;
            if (i == 0) continue;
            
            int wrappedIndex = WrappedIndex(i + carIndex.Index, out int wrapDirection);

            var otherCar = CarEntities[wrappedIndex];

            int otherCarLane = otherCar.Lane;
            float otherCarPositionValue = otherCar.Position;
            
            //Wrap other car pos when appropriate
            if (wrapDirection != 0) otherCarPositionValue += LaneLength * wrapDirection;
            
            int lane = positionInLane.LaneIndex;
            
            bool sameLane = otherCarLane == lane;

            float distance = otherCarPositionValue - positionInLane.Position;
            
            float absoluteDistance = math.abs(distance);

            if (absoluteDistance > CarRadius) //early out if not within importance radius
            {
                continue;
            }
            
            if ((collision.CollisionFlags & CollisionType.Front) == 0 && sameLane && distance > 0) //
            {
                collision.CollisionFlags |= CollisionType.Front;
                
                //TODO what if we didn't care about velocity/dist, used a PID or something in CarDrivingSystem
                //collision.FrontVelocity = CarVelocityLookup[otherCar.Value].VelY;
                collision.FrontDistance = CarRadius - distance; //calc distance to perfect following distance
            }

            if (otherCarLane == lane + 1)
            {
                collision.CollisionFlags |= CollisionType.Right;
            }
            else if (otherCarLane == lane - 1)
            {
                collision.CollisionFlags |= CollisionType.Left;
            }
        }

        //DEBUG
        baseColor.Value = new float4(
            (collision.CollisionFlags & CollisionType.Front) == CollisionType.Front ? 1.0f: 0.0f, 
            (collision.CollisionFlags & CollisionType.Right) == CollisionType.Right ? 1.0f: 0.0f, 
            (collision.CollisionFlags & CollisionType.Left) == CollisionType.Left ? 1.0f: 0.0f, 
            1.0f);
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