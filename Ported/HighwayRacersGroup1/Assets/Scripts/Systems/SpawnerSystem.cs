using System;
using Unity.Collections;
using Unity.Entities;
using Unity.Rendering;
using Unity.Transforms;
using Random = Unity.Mathematics.Random;


partial struct SpawnerSystem : ISystem
{

    public const float minSpeed = 5;
    public const float maxSpeed = 50;

    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<Config>();
    }
    
    public void OnDestroy(ref SystemState state)
    {
    }
    
    public void OnUpdate(ref SystemState state)
    {
        var config = SystemAPI.GetSingleton<Config>();
        
        var random = Random.CreateFromIndex(1234);
        var hue = random.NextFloat();
        
        var ecbSingleton = SystemAPI.GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>();
        var ecb = ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged);

        var segmentPositions = HighwaySpawner.SegmentPositions;
        var segmentDirections = HighwaySpawner.SegmentRightDirections;

        for(var i = 0; i < segmentPositions.Length; ++i)
        {
            var entity = ecb.Instantiate(config.SegmentPrefab);
            var segmentTransform = LocalTransform.FromPosition(segmentPositions[i]);
            ecb.SetComponent(entity, segmentTransform);
            ecb.SetComponent(entity, new Segment { Right = segmentDirections[i]});
        }

        var cars = CollectionHelper.CreateNativeArray<Entity>(config.CarCount, Allocator.Temp);
        ecb.Instantiate(config.CarPrefab, cars);

        var laneIndex = 0;
        var segmentIndex = segmentPositions.Length;


        for (var i = 0; i < cars.Length; ++i)
        {
            //var segmentID = UnityEngine.Random.Range(0, segmentPositions.Length - 1);

            if (i >= segmentIndex)
            {
                segmentIndex += segmentPositions.Length;
                laneIndex++;
            }
                
            var segmentID = i % segmentPositions.Length;

            //var maxSpeed = 50f;
            //var minSpeed = 5f;

            var carSpeed = (UnityEngine.Random.value + (minSpeed / maxSpeed)) * maxSpeed;
            var laneID = laneIndex;

            var car = new CarData
            {
                SegmentID = segmentID,
                Lane = laneID,
                TargetLane = laneID,
                DefaultSpeed = carSpeed,
                Speed = carSpeed,
                inFrontCarIndex = -1
            };
            ecb.SetComponent(cars[i], car);

            float carLane = 4f + (2.45f * car.Lane);
            var position = segmentPositions[segmentID] + (segmentDirections[segmentID] * -carLane);
            ecb.SetComponent(cars[i], LocalTransform.FromPosition(position));
        }

        state.Enabled = false;
    }
}