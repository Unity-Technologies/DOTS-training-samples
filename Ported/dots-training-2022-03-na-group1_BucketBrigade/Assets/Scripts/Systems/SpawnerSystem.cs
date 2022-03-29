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
                var fireLength = spawner.FireDimension * spawner.FireDimension;
                var heatmapBuffer = ecb.AddBuffer<HeatMapTemperature>(heatmapEntity);
                heatmapBuffer.EnsureCapacity(fireLength);
                for (int iFire = 0; iFire < fireLength ; iFire++)//adding elements to buffer
                {
                    heatmapBuffer.Add(new HeatMapTemperature {value = 0});
                }
                
                ecb.AddComponent(heatmapEntity, new HeatMapWidth() { width = spawner.FireDimension });

                var offsetSingleDimension = -(spawner.FireDimension - 1) / 2f;
                var offset = new float2(offsetSingleDimension, offsetSingleDimension);
                
                for (var i = 0; i < spawner.FireDimension; i++)
                {
                    for (var j = 0; j < spawner.FireDimension; j++)
                    {
                        var instance = ecb.Instantiate(spawner.FlameCellPrefab);
                        var position = new Position() {position = offset + new float2(i, j)};
                        ecb.SetComponent(instance, position);
                    }
                }
                
                for (int i = 0; i < spawner.TeamCount; i++)
                {
                    // we have 1 fetcher, 2 captains, and 2 squads.

                    var squadSize = (spawner.MembersCount - 3) / 2;
                    
                    var fetcherEntity = ecb.Instantiate(spawner.FetcherPrefab);
                    ecb.SetComponent(fetcherEntity, new Position {position = new float2(random.NextFloat(offset.x, -offset.x), random.NextFloat(offset.y, -offset.y))});
                    
                    var waterCaptainEntity = ecb.Instantiate(spawner.WaterCaptainPrefab);
                    ecb.SetComponent(waterCaptainEntity, new Position {position = new float2(random.NextFloat(offset.x, -offset.x), random.NextFloat(offset.y, -offset.y))});

                    var fireCaptainEntity = ecb.Instantiate(spawner.FireCaptainPrefab);
                    ecb.SetComponent(fireCaptainEntity, new MyWaterCaptain() {captain = waterCaptainEntity});
                    ecb.SetComponent(fireCaptainEntity, new Position {position = new float2(random.NextFloat(offset.x, -offset.x), random.NextFloat(offset.y, -offset.y))});

                    ecb.SetComponent(fetcherEntity, new DestinationWorker() {worker = waterCaptainEntity});
                    
                    var previousMember = waterCaptainEntity;
                    
                    for (var j = 0; j < squadSize; j++)
                    {
                        var workerEntity = ecb.Instantiate(spawner.FullBucketWorkerPrefab);
                        ecb.SetComponent(workerEntity, new RelocationPosition() { positionAlongSpline = (float)(j + 1) / (float)(squadSize + 1)});
                        ecb.SetComponent(workerEntity, new MyWaterCaptain() {captain = waterCaptainEntity});
                        ecb.SetComponent(workerEntity, new MyFireCaptain() {captain = fireCaptainEntity});
                        ecb.SetComponent(workerEntity, new Position {position = new float2(random.NextFloat(offset.x, -offset.x), random.NextFloat(offset.y, -offset.y))});
                        
                        ecb.SetComponent(previousMember, new DestinationWorker() {worker = workerEntity});

                        previousMember = workerEntity;
                    }
                    
                    ecb.SetComponent(previousMember, new DestinationWorker() {worker = fireCaptainEntity});

                    previousMember = fireCaptainEntity;
                    
                    for (var j = 0; j < squadSize; j++)
                    {
                        var workerEntity = ecb.Instantiate(spawner.EmptyBucketWorkerPrefab);
                        ecb.SetComponent(workerEntity, new RelocationPosition() { positionAlongSpline = (float)(j + 1) / (float)(squadSize + 1)});
                        ecb.SetComponent(workerEntity, new MyWaterCaptain() {captain = waterCaptainEntity});
                        ecb.SetComponent(workerEntity, new MyFireCaptain() {captain = fireCaptainEntity});
                        ecb.SetComponent(workerEntity, new Position {position = new float2(random.NextFloat(offset.x, -offset.x), random.NextFloat(offset.y, -offset.y))});
                        
                        ecb.SetComponent(previousMember, new DestinationWorker() {worker = workerEntity});
                        
                        previousMember = workerEntity;
                    }

                    ecb.SetComponent(previousMember, new DestinationWorker() {worker = waterCaptainEntity});
                }

                for (int i = 0; i < spawner.BucketCount; i++)
                {
                    entity = ecb.Instantiate(spawner.BucketPrefab);
                    ecb.SetComponent(entity, new Position {position = new float2(random.NextFloat(offset.x, -offset.x), random.NextFloat(offset.y, -offset.y))});
                }

                var waterCountEachSide = spawner.WaterCount / 4;
                var waterDistance = spawner.FireDimension / (float)(waterCountEachSide - 1);
                for (int i = 0; i < waterCountEachSide; i++)
                {
                    entity = ecb.Instantiate(spawner.WaterPoolPrefab);
                    ecb.SetComponent(entity, new Position() {position = new float2(offset.x - 3, offset.y + waterDistance * i)});
                    ecb.SetComponent(entity, new Capacity() { amount = random.NextInt(spawner.MinWaterSupplyCount, spawner.MaxWaterSupplyCount)});
                }
                
                for (int i = 0; i < waterCountEachSide; i++)
                {
                    entity = ecb.Instantiate(spawner.WaterPoolPrefab);
                    ecb.SetComponent(entity, new Position {position = new float2(-(offset.x - 3), offset.y + waterDistance * i)});
                    ecb.SetComponent(entity, new Capacity() { amount = random.NextInt(spawner.MinWaterSupplyCount, spawner.MaxWaterSupplyCount)});
                }
                
                for (int i = 0; i < waterCountEachSide; i++)
                {
                    entity = ecb.Instantiate(spawner.WaterPoolPrefab);
                    ecb.SetComponent(entity, new Position {position = new float2(offset.x + waterDistance * i, offset.y - 3)});
                    ecb.SetComponent(entity, new Capacity() { amount = random.NextInt(spawner.MinWaterSupplyCount, spawner.MaxWaterSupplyCount)});
                }
                
                for (int i = 0; i < waterCountEachSide; i++)
                {
                    entity = ecb.Instantiate(spawner.WaterPoolPrefab);
                    ecb.SetComponent(entity, new Position {position = new float2(offset.x + waterDistance * i, -(offset.y - 3))});
                    ecb.SetComponent(entity, new Capacity() { amount = random.NextInt(spawner.MinWaterSupplyCount, spawner.MaxWaterSupplyCount)});
                }
            }).Run();

        ecb.Playback(EntityManager);
        ecb.Dispose();
    }
}
