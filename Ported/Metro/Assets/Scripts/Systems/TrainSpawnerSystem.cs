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
            // for (int i = 0; i < 1; i++)
            {
                Entity firstTrain = ecb.Instantiate(trainSpawner.CarriagePrefab);
                Entity previousTrain = firstTrain;
                
                ref var line = ref metroBlob.Blob.Value.Lines[i];
                for (int j = line.Path.Length - 1; j > 0; j--)
                // for (int j = 5; j > 1; j--)
                {
                    ref var point = ref line.Path[j];
            
                    var trainEntity = ecb.Instantiate(trainSpawner.CarriagePrefab);
                    ecb.SetComponent(trainEntity, new Carriage()
                    {
                        num = j,
                        NextTrain = previousTrain,
                        LaneIndex = i,
                        NextPlatformIndex = BezierUtilities.Get_NextPlatformIndex(point.distanceAlongPath, -1, ref metroBlob.Blob.Value, i),
                        PositionAlongTrack = point.distanceAlongPath
                    });
            
                    previousTrain = trainEntity;
                }
                
                ecb.SetComponent(firstTrain, new Carriage()
                {
                    num = 0,
                    NextTrain = previousTrain,
                    LaneIndex = i,
                    NextPlatformIndex = BezierUtilities.Get_NextPlatformIndex(line.Path[0].distanceAlongPath, -1, ref metroBlob.Blob.Value, i),
                    PositionAlongTrack = line.Path[0].distanceAlongPath
                });
            }
            
        }).Run();

        ecb.Playback(EntityManager);
        ecb.Dispose();
    }
}
