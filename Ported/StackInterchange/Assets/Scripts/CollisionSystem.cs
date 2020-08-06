using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Networking;

public class CollisionSystem : SystemBase
{
    private const float BrakingFactor = 0.9f;
    
    private EntityQuery m_CarQuery;
    
    private struct SegmentCar
    {
        public int segmentIndex;
        public Entity carEntity;
    }

    private struct CarRange
    {
        public int Offset;
        public int Count;
    }

    private struct SortBySegment : IComparer<SegmentCar>
    {
        public int Compare(SegmentCar x, SegmentCar y)
        {
            if (x.segmentIndex < y.segmentIndex) return -1;
            if (y.segmentIndex < x.segmentIndex) return 1;
            if (x.carEntity.Index < y.carEntity.Index) return -1;
            if (y.carEntity.Index < x.carEntity.Index) return 1;
            return 0;
        }
    }

    protected override void OnCreate()
    {
        m_CarQuery = GetEntityQuery(new EntityQueryDesc
        {
            All = new[]
            {
                ComponentType.ReadOnly<BelongToSpline>()
            }
        });
    }

    // TODO: This should be a generic helper in one place
    // TODO: Offset should be taken into account
    private static float3 CalculateCarPosition(in SegmentCollection segmentCollection, 
        in CurrentSegment currentSegment, in Progress progress, in Offset offset)
    {
        var segment = segmentCollection.Value.Value.Segments[currentSegment.Value];
        var position = math.lerp(segment.Start, segment.End, progress.Value);
        return position;
    }

    // TODO: This should be a generic helper in one place
    // TODO: Offset should be taken into account
    private static float3 CalculateCarDirection(in SegmentCollection segmentCollection,
        in CurrentSegment currentSegment, in Progress progress, in Offset offset)
    {
        var segment = segmentCollection.Value.Value.Segments[currentSegment.Value];
        var forward = segment.End - segment.Start;
        return forward;
    }

    protected override void OnUpdate()
    {
        // 1. Sort cars by segment in one big array
        int numberOfCars = m_CarQuery.CalculateEntityCount();
        NativeArray<SegmentCar> carList = new NativeArray<SegmentCar>(numberOfCars, Allocator.Temp);
        
        Entities
            .ForEach((int entityInQueryIndex, in Entity entity, in CurrentSegment currentSegment) =>
            {
                carList[entityInQueryIndex] = new SegmentCar{ carEntity = entity, segmentIndex = currentSegment.Value };    
            }).ScheduleParallel();

        var sortBySegment = new SortBySegment();
        carList.Sort(sortBySegment);

        // 2. Find the start offset and number of cars for each segment in the array
        var segmentCollection = GetSingleton<SegmentCollection>();
        int numberOfSegments = segmentCollection.Value.Value.Segments.Length;
        NativeArray<CarRange> carsRangePerSegment = new NativeArray<CarRange>(numberOfSegments, Allocator.Temp);
        int currentCarIndex = 0;
        for (int i = 0; i < numberOfSegments; ++i)
        {
            int start = currentCarIndex;
            int count = 0;
            while (currentCarIndex >= numberOfCars && carList[currentCarIndex].segmentIndex == i)
            {
                count++;
                currentCarIndex++;
            }

            carsRangePerSegment[i] = new CarRange {Count = count, Offset = start };
        }

        // 3. For each car find the collisions with the cars in the same segment
        Entities
            .ForEach((
                ref Speed speed, 
                in Entity thisCarEntity,
                in Size size,
                in Offset offset,
                in CurrentSegment currentSegment,
                in Progress progress) =>
            {
                var position = CalculateCarPosition(segmentCollection, currentSegment, progress, offset);
                var direction = CalculateCarDirection(segmentCollection, currentSegment, progress, offset);

                // TODO: Make sure this is update the same way as CarMovementSystem does it
                var nextPosition = position + speed.Value * direction;

                bool willCollide = false;
                
                // We test against a few cars nearby instead of all of them
                CarRange carsToTest = carsRangePerSegment[currentSegment.Value];
                for (int i = carsToTest.Offset; i < carsToTest.Offset + carsToTest.Count; ++i)
                {
                    Entity theOtherCarEntity = carList[i].carEntity;
                    if (theOtherCarEntity == thisCarEntity) continue; // Don't compare the car against itself

                    Size theOtherCarSize = GetComponent<Size>(theOtherCarEntity);
                    Offset theOtherCarOffset = GetComponent<Offset>(theOtherCarEntity);
                    CurrentSegment theOtherCarCurrentSegment = GetComponent<CurrentSegment>(theOtherCarEntity);
                    Progress theOtherCarProgress = GetComponent<Progress>(theOtherCarEntity);
                    
                    var theOtherCarPosition = CalculateCarPosition(segmentCollection, 
                        theOtherCarCurrentSegment,
                        theOtherCarProgress,
                        theOtherCarOffset);
                    var theOtherCarDirection = CalculateCarDirection(segmentCollection,
                        theOtherCarCurrentSegment,
                        theOtherCarProgress,
                        theOtherCarOffset);
                    
                    // TODO: Make sure this is update the same way as CarMovementSystem does it
                    var theOtherCarNextPosition = position + speed.Value * direction;
                    
                    // Simply assume that if the car is in front of as and the next positions will be closer than the
                    // car maximum extents together, then there can be a collision.
                    float3 diff = theOtherCarNextPosition - nextPosition;
                    bool theOtherCarIsInFront = math.dot(direction, diff) > 0;

                    // The car in the back should do the braking, not the front one
                    if (theOtherCarIsInFront)
                    {
                        float distance = math.length(diff);
                        float maxExtent = math.length(size.Value) + math.length(theOtherCarSize.Value);
                        if (distance < maxExtent)
                        {
                            willCollide = true;
                            break;
                        }
                    }
                }
                
                if (willCollide)
                {
                    // Avoid collision by slowing down
                    speed.Value *= BrakingFactor;
                }
                else
                {
                    // TODO: Detect the situation, where we have slowed down, but the road is clear again,
                    //       and we could accelerate to the original speed.
                }
            }).ScheduleParallel();
    }
}
