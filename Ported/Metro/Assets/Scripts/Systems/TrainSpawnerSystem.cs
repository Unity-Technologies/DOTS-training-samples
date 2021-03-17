using src.DOTS.Components;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;

public class TrainSpawnerSystem : SystemBase
{
    protected override void OnUpdate()
    {
        var ecb = new EntityCommandBuffer(Allocator.Temp);
        var metroBlob = this.GetSingleton<MetroBlobContaner>();

        Entities.WithoutBurst().ForEach((Entity entity, ref TrainSpawner trainSpawner) =>
        {
            ecb.DestroyEntity(entity);
            
            // Spawning rails
            for (int i = 0; i < metroBlob.Blob.Value.Lines.Length; i++)
            {
                float _DIST = 0f;
                while (_DIST < metroBlob.Blob.Value.Lines[i].Distance)
                {
                    float _DIST_AS_RAIL_FACTOR = _DIST / metroBlob.Blob.Value.Lines[i].Distance;
                    float3 _RAIL_POS = BezierUtilities.Get_Position(_DIST_AS_RAIL_FACTOR, ref metroBlob.Blob.Value.Lines[i]);
                    float3 _RAIL_ROT = BezierUtilities.Get_NormalAtPosition(_DIST_AS_RAIL_FACTOR, ref metroBlob.Blob.Value.Lines[i]);
                    
                    var railEntity = ecb.Instantiate(trainSpawner.RailPrefab);
                    ecb.AddComponent(railEntity, new Rotation(){Value = 
                        quaternion.LookRotation(_RAIL_ROT, new float3(0.0f, 1.0f, 0.0f)) });
                    ecb.AddComponent(railEntity, new Translation(){Value = _RAIL_POS});
                    
                    _DIST += Metro.RAIL_SPACING;
                }
            }

            // Spawning Trains
            for (int i = 0; i < metroBlob.Blob.Value.Lines.Length; i++)
            {
                Entity firstTrain = ecb.Instantiate(trainSpawner.CarriagePrefab);
                Entity previousTrain = firstTrain;

                var distanceBetweenTrains = 1f / trainSpawner.TrainsPerLine;
                var position = 1f - distanceBetweenTrains;

                while (position > 0)
                {
                    var trainEntity = ecb.Instantiate(trainSpawner.CarriagePrefab);
                    ecb.SetComponent(trainEntity, new Carriage()
                    {
                        NextTrain = previousTrain,
                        LaneIndex = i,
                        NextPlatformIndex = BezierUtilities.Get_NextPlatformIndex(position, -1, ref metroBlob.Blob.Value, i),
                        PositionAlongTrack = position
                    });
            
                    previousTrain = trainEntity;
                    position -= distanceBetweenTrains;
                }
                
                ecb.SetComponent(firstTrain, new Carriage()
                {
                    NextTrain = previousTrain,
                    LaneIndex = i,
                    NextPlatformIndex = BezierUtilities.Get_NextPlatformIndex(0, -1, ref metroBlob.Blob.Value, i),
                    PositionAlongTrack = 0
                });
            }
            
        }).Run();

        ecb.Playback(EntityManager);
        ecb.Dispose();
    }
}
