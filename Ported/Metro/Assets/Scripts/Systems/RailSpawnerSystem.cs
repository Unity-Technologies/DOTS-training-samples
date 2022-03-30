using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

public partial class RailSpawnerSystem : SystemBase
{
    
    // Run ONLY if RailSpawnerComponent exists
    protected override void OnUpdate()
    {
        EntityCommandBuffer ecb = new EntityCommandBuffer(Allocator.TempJob);
        
        Entity trackPrefab = new Entity();
        Entity platformPrefab = new Entity();
        Entity carriagePrefab = new Entity();
        float railSpacing = 0;
        
        Entities.ForEach((Entity entity, in RailSpawnerComponent railSpawner) =>
        {
            trackPrefab = railSpawner.TrackPrefab;
            platformPrefab = railSpawner.PlatformPrefab;
            carriagePrefab = railSpawner.CarriagePrefab;
            railSpacing = railSpawner.RailSpacing;
            
            ecb.DestroyEntity(entity);
        }).Run();

        Entities.ForEach((Entity Entity, in LineComponent lineComponent,
            in DynamicBuffer<BezierPointBufferElement> bezierPoints,
            in LineTotalDistanceComponent lineTotalDistanceComponent) =>
        {
            float lineLength = lineTotalDistanceComponent.Value;
            
            for (float i = 0; i * railSpacing < lineLength; i++)
            {
                float t = i / lineLength;
                Debug.Log(trackPrefab);
                if (trackPrefab == Entity.Null) // DIRTY HACK
                    break;
                
                var instance = ecb.Instantiate(trackPrefab);
                float3 position = BezierHelpers.GetPosition(bezierPoints, lineLength, t);
                var translation = new Translation { Value = position};
                ecb.SetComponent(instance, translation);

                quaternion normal = quaternion.Euler(BezierHelpers.GetNormalAtPosition(bezierPoints, lineLength, t));
                var rotation = new Rotation { Value = normal };
                ecb.SetComponent(instance, rotation);
            }
        }).Run();
        
        ecb.Playback(EntityManager);
        ecb.Dispose();
    }
}
