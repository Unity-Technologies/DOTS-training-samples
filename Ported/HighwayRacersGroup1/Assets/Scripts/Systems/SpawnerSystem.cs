using System;
using Unity.Collections;
using Unity.Entities;
using Unity.Rendering;
using Unity.Transforms;
using Random = Unity.Mathematics.Random;


partial struct SpawnerSystem : ISystem
{
    private EntityQuery m_BaseColorQuery;
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<Config>();

        m_BaseColorQuery = state.GetEntityQuery(ComponentType.ReadOnly<URPMaterialPropertyBaseColor>());
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
        
        URPMaterialPropertyBaseColor RandomColor()
        {
            hue = (hue + 0.618034005f) % 1;
            var color = UnityEngine.Color.HSVToRGB(hue, 1.0f, 1.0f);
            return new URPMaterialPropertyBaseColor { Value = (UnityEngine.Vector4)color };
        }
        
        var queryMask = m_BaseColorQuery.GetEntityQueryMask();
        for (var i = 0; i < cars.Length; ++i)
        {
            var segmentID = UnityEngine.Random.Range(0, segmentPositions.Length - 1);
            var car = new CarData
            {
                SegmentID = segmentID, Lane = UnityEngine.Random.Range(0, 4),
                Speed = UnityEngine.Random.value * 50f,
                DistanceToCarInFront = 5f
            };
            ecb.SetComponent(cars[i], car);
            var position = segmentPositions[segmentID];
            // todo: add offset between segments
            ecb.SetComponent(cars[i], LocalTransform.FromPosition(position));
            
            // Set a random color to each car entity
            ecb.SetComponentForLinkedEntityGroup(cars[i], queryMask, RandomColor());
        }

        state.Enabled = false;
    }
}