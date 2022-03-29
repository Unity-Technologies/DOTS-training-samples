using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;
using Unity.Transforms;

public partial class SpawnerSystem : SystemBase
{
    protected override void OnUpdate()
    {
        var ecb = new EntityCommandBuffer(Allocator.Temp);

        // The PRNG (pseudorandom number generator) from Unity.Mathematics is a struct
        // and can be used in jobs. For simplicity and debuggability in development,
        // we'll initialize it with a constant. (In release, we'd want a seed that
        // randomly varies, such as the time from the user's system clock.)
        var random = new Random(1234);

        Entities
            .ForEach((Entity entity, in Spawner spawner) =>
            {
                ecb.DestroyEntity(entity);

                var heatmapEntity = ecb.CreateEntity();
                ecb.SetName(heatmapEntity, "Fire");
                var heatmapBuffer = ecb.AddBuffer<HeatMapTemperature>(heatmapEntity);
                for (int iFire = 0; iFire < spawner.FireDimension ; iFire++)//adding elements to buffer
                {
                    heatmapBuffer.Add(new HeatMapTemperature {value = 0});
                }
                
                ecb.AddComponent(heatmapEntity, new HeatMapWidth() { width = spawner.FireDimension });

                var offsetSingleDimension = -(spawner.FireDimension - 1) / 2f;
                var offset = new float3(offsetSingleDimension, 0f, offsetSingleDimension);
                
                for (var i = 0; i < spawner.FireDimension; i++)
                {
                    for (var j = 0; j < spawner.FireDimension; j++)
                    {
                        var instance = ecb.Instantiate(spawner.FlameCellPrefab);
                        var translation = new Translation {Value = offset + new float3(i, 0,j)};
                        ecb.SetComponent(instance, translation);
                    }
                }
                for (int i = 0; i < spawner.TeamCount; i++)
                {
                    // we have 1 fetcher, 2 captains, and 2 squads.

                    var squadSize = (spawner.MembersCount - 3) / 2;
                    
                    var fetcherEntity = ecb.Instantiate(spawner.BotPrefab);
                    ecb.SetName(fetcherEntity, "Fetcher");
                    ecb.AddComponent<FetcherTag>(fetcherEntity);
                    
                    var waterCaptainEntity = ecb.Instantiate(spawner.BotPrefab);
                    ecb.SetName(waterCaptainEntity, "WaterCaptain");
                    ecb.AddComponent<WaterCaptainTag>(waterCaptainEntity);
                    ecb.AddComponent<SourcePool>(waterCaptainEntity);
                    ecb.AddComponent<SourcePosition>(waterCaptainEntity);
                    
                    var fireCaptainEntity = ecb.Instantiate(spawner.BotPrefab);
                    ecb.SetName(fireCaptainEntity, "FireCaptain");
                    ecb.AddComponent<FireCaptainTag>(fireCaptainEntity);
                    ecb.SetComponent(fireCaptainEntity, new MyWaterCaptain() {captain = waterCaptainEntity});
                    
                    ecb.SetComponent(fetcherEntity, new DestinationWorker() {worker = waterCaptainEntity});
                    
                    var previousMember = waterCaptainEntity;
                    
                    for (var j = 0; j < squadSize; j++)
                    {
                        var workerEntity = ecb.Instantiate(spawner.BotPrefab);
                        ecb.SetName(workerEntity, "WaterHauler");
                        ecb.AddComponent<WorkerTag>(workerEntity);
                        ecb.AddComponent<FullBucketWorkerTag>(workerEntity);
                        ecb.AddComponent(workerEntity, new RelocationPosition() { positionAlongSpline = (float)(j + 1) / (float)(squadSize + 1)});

                        ecb.SetComponent(previousMember, new DestinationWorker() {worker = workerEntity});
                        ecb.SetComponent(workerEntity, new MyWaterCaptain() {captain = waterCaptainEntity});
                        ecb.SetComponent(workerEntity, new MyFireCaptain() {captain = fireCaptainEntity});

                        previousMember = workerEntity;
                    }
                    
                    ecb.SetComponent(previousMember, new DestinationWorker() {worker = fireCaptainEntity});

                    previousMember = fireCaptainEntity;
                    
                    for (var j = 0; j < squadSize; j++)
                    {
                        var workerEntity = ecb.Instantiate(spawner.BotPrefab);
                        ecb.SetName(workerEntity, "BucketHauler");
                        ecb.AddComponent<WorkerTag>(workerEntity);
                        ecb.AddComponent<EmptyBucketWorkerTag>(workerEntity);
                        ecb.AddComponent(workerEntity, new RelocationPosition() { positionAlongSpline = (float)(j + 1) / (float)(squadSize + 1)});
                        ecb.SetComponent(workerEntity, new MyWaterCaptain() {captain = waterCaptainEntity});
                        ecb.SetComponent(workerEntity, new MyFireCaptain() {captain = fireCaptainEntity});
                        
                        ecb.SetComponent(previousMember, new DestinationWorker() {worker = workerEntity});

                        previousMember = workerEntity;
                    }

                    ecb.SetComponent(previousMember, new DestinationWorker() {worker = waterCaptainEntity});
                }
                
                // spawn water
            }).Run();

        ecb.Playback(EntityManager);
        ecb.Dispose();
    }
}
