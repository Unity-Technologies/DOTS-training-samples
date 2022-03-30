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
            heatmapBuffer.Add(new HeatMapTemperature {value = 0});
        }

        ecb.AddComponent(heatmapEntity, new HeatMapData()
        {
            mapSideLength = size, 
            maxTileHeight = 5.0f,
            
            heatPropagationSpeed = 0.05f,//original is 0.0003
            heatPropagationRadius = 2,

            colorNeutral = new float4(0.49f,0.8f,0.46f,1f),
            colorCool = new float4(1f,1f,0.5f,1f),
            colorHot = new float4(1f,0f,0f,1f)
        });
    }
    
    static void SpawnSplashmap(EntityCommandBuffer ecb, int size)
    {
        var splashmapEntity = ecb.CreateEntity();
        ecb.SetName(splashmapEntity, "Splashmap");
        
        var heatmapBuffer = ecb.AddBuffer<HeatMapSplash>(splashmapEntity);
        for (int iSplash = 0; iSplash < size ; iSplash++)//adding elements to buffer
        {
            heatmapBuffer.Add(new HeatMapSplash {value = -1});
        }
    }

    static void SpawnFireColumns(EntityCommandBuffer ecb, Entity firePrefab, int size)
   {
       var offsetSingleDimension = -(size - 1) / 2f;
       var offset = new float3(offsetSingleDimension , 0, offsetSingleDimension);
                
       for (var x = 0; x < size; x++)
       {
           for (var z = 0; z < size; z++)
           {
               var instance = ecb.Instantiate(firePrefab);
               ecb.SetComponent(instance, new Translation()
               {
                   Value = offset + new float3(x, 0f, z)
               });
               
               ecb.SetComponent(instance, new URPMaterialPropertyBaseColor
               {
                   Value = new float4(1,1,1,1)
               });
               ecb.SetComponent(instance, new FireIndex
               {
                   index = x + z *size
               });
           }
       }
   }

   void StartRandomFires(int amountFires)
   {
       var heatmapEntity = GetSingletonEntity<HeatMapTemperature>();
       DynamicBuffer<HeatMapTemperature> heatmapBuffer = EntityManager.GetBuffer<HeatMapTemperature>(heatmapEntity);
       Random random = new Random(123123);

       // Set random tiles on fire by default
       for (int i = 0; i < amountFires; i++)
       {
           int randomIndex = random.NextInt(0, heatmapBuffer.Length);

           heatmapBuffer[randomIndex] = 0.2f;
       }
   }
   
   static void SpawnTeams(EntityCommandBuffer ecb, Spawner spawner, Random random)
   {
       var radius = (spawner.FireDimension - 1) / 2f;
                
            for (int i = 0; i < spawner.TeamCount; i++)
            {
                // we have 1 fetcher, 2 captains, and 2 squads.

                var squadSize = (spawner.MembersCount - 3) / 2;
                
                var fetcherEntity = ecb.Instantiate(spawner.FetcherPrefab);
                ecb.SetComponent(fetcherEntity, new Position {Value = new float2(random.NextFloat(-radius, radius), random.NextFloat(-radius, radius))});
                ecb.SetComponent(fetcherEntity, new Speed() { Value = 4f });
                
                var waterCaptainEntity = ecb.Instantiate(spawner.WaterCaptainPrefab);
                ecb.SetComponent(waterCaptainEntity, new Position {Value = new float2(random.NextFloat(-radius, radius), random.NextFloat(-radius, radius))});
                ecb.SetComponent(waterCaptainEntity, new Speed() { Value = 4f });

                var fireCaptainEntity = ecb.Instantiate(spawner.FireCaptainPrefab);
                ecb.SetComponent(fireCaptainEntity, new MyWaterCaptain() {Value = waterCaptainEntity});
                ecb.SetComponent(fireCaptainEntity, new Position {Value = new float2(random.NextFloat(-radius, radius), random.NextFloat(-radius, radius))});
                ecb.SetComponent(fireCaptainEntity, new Speed() { Value = 4f });

                ecb.SetComponent(fetcherEntity, new DestinationWorker() {Value = waterCaptainEntity});
                
                var previousMember = waterCaptainEntity;
                
                for (var j = 0; j < squadSize; j++)
                {
                    var workerEntity = ecb.Instantiate(spawner.FullBucketWorkerPrefab);
                    ecb.SetComponent(workerEntity, new RelocationPosition() { Value = (float)(j + 1) / (float)(squadSize + 1)});
                    ecb.SetComponent(workerEntity, new MyWaterCaptain() {Value = waterCaptainEntity});
                    ecb.SetComponent(workerEntity, new MyFireCaptain() {Value = fireCaptainEntity});
                    ecb.SetComponent(workerEntity, new Position {Value = new float2(random.NextFloat(-radius, radius), random.NextFloat(-radius, radius))});
                    ecb.SetComponent(workerEntity, new Speed() { Value = 4f });
                    
                    ecb.SetComponent(previousMember, new DestinationWorker() {Value = workerEntity});

                    previousMember = workerEntity;
                }
                
                ecb.SetComponent(previousMember, new DestinationWorker() {Value = fireCaptainEntity});

                previousMember = fireCaptainEntity;
                
                for (var j = 0; j < squadSize; j++)
                {
                    var workerEntity = ecb.Instantiate(spawner.EmptyBucketWorkerPrefab);
                    ecb.SetComponent(workerEntity, new RelocationPosition() { Value = (float)(j + 1) / (float)(squadSize + 1)});
                    ecb.SetComponent(workerEntity, new MyWaterCaptain() {Value = waterCaptainEntity});
                    ecb.SetComponent(workerEntity, new MyFireCaptain() {Value = fireCaptainEntity});
                    ecb.SetComponent(workerEntity, new Position {Value = new float2(random.NextFloat(-radius, radius), random.NextFloat(-radius, radius))});
                    ecb.SetComponent(workerEntity, new Speed() { Value = 4f });
                    
                    ecb.SetComponent(previousMember, new DestinationWorker() {Value = workerEntity});
                    
                    previousMember = workerEntity;
                }

                ecb.SetComponent(previousMember, new DestinationWorker() {Value = waterCaptainEntity});
            }
   }

   static void SpawnOmniworkers(EntityCommandBuffer ecb, Entity prefab, int count, int size, Random random)
   {
       var radius = (size - 1) / 2f;

       for (int i = 0; i < count; i++)
       {
           var entity = ecb.Instantiate(prefab);
           ecb.SetComponent(entity, new Position {Value = new float2(random.NextFloat(-radius, radius), random.NextFloat(-radius, radius))});
           ecb.SetComponent(entity, new Speed() { Value = 4f });
       }
   }

   static void SpawnBuckets(EntityCommandBuffer ecb, Entity prefab, int count, int size, Random random)
   {
       var radius = (size - 1) / 2f;

       for (int i = 0; i < count; i++)
       {
           var entity = ecb.Instantiate(prefab);
           ecb.SetComponent(entity, new Position {Value = new float2(random.NextFloat(-radius, radius), random.NextFloat(-radius, radius))});
       }
   }
   
   static void SpawnWaterPools(EntityCommandBuffer ecb, Entity prefab, int count, int size, int minWater, int maxWater, Random random)
   {
       var radius = (size - 1) / 2f;
       var distanceFromEdge = 3;

       var waterCountEachSide = count / 4;
       var waterDistance = radius * 2f / (float)(waterCountEachSide - 1);

       for (int i = 0; i < waterCountEachSide; i++)
       {
           var entity = ecb.Instantiate(prefab);
           ecb.SetComponent(entity, new Position() {Value = new float2(-radius - distanceFromEdge, -radius + waterDistance * i)});
           ecb.SetComponent(entity, new Capacity() { Value = random.NextInt(minWater, maxWater)});
       }
                
       for (int i = 0; i < waterCountEachSide; i++)
       {
           var entity = ecb.Instantiate(prefab);
           ecb.SetComponent(entity, new Position {Value = new float2(radius + distanceFromEdge, -radius + waterDistance * i)});
           ecb.SetComponent(entity, new Capacity() { Value = random.NextInt(minWater, maxWater)});
       }
                
       for (int i = 0; i < waterCountEachSide; i++)
       {
           var entity = ecb.Instantiate(prefab);
           ecb.SetComponent(entity, new Position {Value = new float2(-radius + waterDistance * i, -radius - distanceFromEdge)});
           ecb.SetComponent(entity, new Capacity() { Value = random.NextInt(minWater, maxWater)});
       }
                
       for (int i = 0; i < waterCountEachSide; i++)
       {
           var entity = ecb.Instantiate(prefab);
           ecb.SetComponent(entity, new Position {Value = new float2(-radius + waterDistance * i, radius + distanceFromEdge)});
           ecb.SetComponent(entity, new Capacity() { Value = random.NextInt(minWater, maxWater)});
       }
   }
   
   static void SpawnGround(EntityCommandBuffer ecb, Entity prefab, int size)
   {
       var entity = ecb.Instantiate(prefab);
       ecb.SetComponent(entity, new NonUniformScale() {Value = new float3(size, 0.01f, size)});
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
                
                SpawnHeatmap(ecb, spawner.FireDimension);
                
                SpawnSplashmap(ecb, spawner.BucketCount);

                SpawnFireColumns(ecb, spawner.FlameCellPrefab, spawner.FireDimension);
                
                SpawnTeams(ecb, spawner, random);
                
                SpawnOmniworkers(ecb, spawner.OmniWorkerPrefab, spawner.OmniWorkerCount, spawner.FireDimension, random);

                SpawnBuckets(ecb, spawner.BucketPrefab, spawner.BucketCount, spawner.FireDimension, random);

                SpawnWaterPools(ecb, spawner.WaterPoolPrefab, spawner.WaterCount, spawner.FireDimension, spawner.MinWaterSupplyCount, spawner.MaxWaterSupplyCount, random);

                //SpawnGround(ecb, spawner.GroundPrefab, spawner.FireDimension);
                
            }).Run();

        ecb.Playback(EntityManager);
        ecb.Dispose();

        StartRandomFires(5);

        this.Enabled = false;
    }
}
