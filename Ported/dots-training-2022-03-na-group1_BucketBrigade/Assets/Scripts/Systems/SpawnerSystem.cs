using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;
using Unity.Transforms;

public partial class SpawnerSystem : SystemBase
{
   static void SpawnHeatmap(EntityCommandBuffer ecb, int size)
    {
        var heatmapEntity = ecb.CreateEntity();
        ecb.SetName(heatmapEntity, "Heatmap");
        var heatmapBuffer = ecb.AddBuffer<HeatMapTemperature>(heatmapEntity);
        for (int iFire = 0; iFire < size * size ; iFire++)//adding elements to buffer
        {
            heatmapBuffer.Add(new HeatMapTemperature {value = 0.2f});
        }
                
        ecb.AddComponent(heatmapEntity, new HeatMapData()
        {
            width = size , 
            heatSpeed = 0.01f,
            startColor = new float4(0f,0f,0f,1f),
            finalColor = new float4(0f,0f,0f,1f)
        });
    }

   static void SpawnFireColumns(EntityCommandBuffer ecb, Entity firePrefab, int size)
   {
       var offsetSingleDimension = -(size - 1) / 2f;
       var offset = new float3(offsetSingleDimension, 0f, offsetSingleDimension);
                
       for (var i = 0; i < size; i++)
       {
           for (var j = 0; j < size; j++)
           {
               var instance = ecb.Instantiate(firePrefab);
               ecb.SetComponent(instance, new Translation
               {
                   Value = offset + new float3(i, 0,j)
               });
               ecb.SetComponent(instance, new URPMaterialPropertyBaseColor
               {
                   Value = new float4(1,1,1,1)
               });
               ecb.SetComponent(instance, new FireIndex
               {
                   index = i + j *size
               });
           }
       }
   }
   
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
                
                SpawnHeatmap(ecb,spawner.FireDimension);

                SpawnFireColumns(ecb, spawner.FlameCellPrefab, spawner.FireDimension);
                
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
