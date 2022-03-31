using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;
using Unity.Transforms;
using static BucketBrigadeUtility;

[UpdateInGroup(typeof(SimulationSystemGroup), OrderFirst = true)]
public partial class SpawnerSystem : SystemBase
{
    private const int MaxEvalOffset = 59;

    static void SpawnCommunicationBuffers(EntityCommandBuffer ecb)
    {
        var helperEntity = ecb.CreateEntity();
        ecb.AddBuffer<FreeBucketInfo>(helperEntity);
        ecb.AddBuffer<WaterPoolInfo>(helperEntity);
        ecb.AddBuffer<DropBucketCommand>(helperEntity);
        ecb.AddBuffer<DumpBucketCommand>(helperEntity);
        ecb.AddBuffer<FillBucketCommand>(helperEntity);
        ecb.AddBuffer<PickupBucketCommand>(helperEntity);
        ecb.AddBuffer<TeamReformCommand>(helperEntity);
    }
    
    static void SpawnHeatmap(EntityCommandBuffer ecb, int size, float propagationSpeed)
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
            
            heatPropagationSpeed = propagationSpeed,//original is 0.0003
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
       var offsetSingleDimension = (size - 1) * 0.5f;
       var offset = new float3(offsetSingleDimension , 0, offsetSingleDimension);
                
       for (var z = 0; z < size; z++)
       {
           for (var x = 0; x < size; x++)
           {
               var instance = ecb.Instantiate(firePrefab);
               ecb.SetComponent(instance, new Translation()
               {
                   Value = new float3(x, 0f, z) - offset
               });
               
               ecb.SetComponent(instance, new URPMaterialPropertyBaseColor
               {
                   Value = new float4(1,1,1,1)
               });
               ecb.SetComponent(instance, new FireIndex
               {
                   //index = z + x *size
                   index = (z *size) + x
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

   static Entity SpawnWorker(EntityCommandBuffer ecb, Entity prefab, Position postion)
   {
       var entity = ecb.Instantiate(prefab);
       ecb.SetComponent(entity, postion);
       ecb.SetComponent(entity, new Home()
       {
           Value = postion.Value
       });
       ecb.SetComponent(entity, new RelocatePosition()
       {
           Value = postion.Value
       });
       ecb.SetComponent(entity, new Speed() { Value = FreeSpeed });

       return entity;
   }

   static Position GenFieldPos(Random random, float radius)
   {
       return new Position { Value = new float2(random.NextFloat(-radius, radius), random.NextFloat(-radius, radius)) };
   }
   
   static void SpawnTeams(EntityCommandBuffer ecb, Spawner spawner, Random random)
   {
       var radius = (spawner.FireDimension - 1) / 2f;
                
            for (int i = 0; i < spawner.TeamCount; i++)
            {
                var squadSize = (spawner.MembersCount - 2) / 2;

                var captainEntity = SpawnWorker(ecb, spawner.CaptainPrefab, GenFieldPos(random, radius));
                ecb.SetComponent(captainEntity, new MyTeam() { Value = captainEntity });
                ecb.SetComponent(captainEntity, new EvalOffsetFrame() { Value = random.NextInt(0, MaxEvalOffset) });
                ecb.AddBuffer<Member>(captainEntity);
                
                var fetcherEntity = SpawnWorker(ecb, spawner.FetcherPrefab, GenFieldPos(random, radius));
                ecb.SetComponent(fetcherEntity, new MyTeam() { Value = captainEntity });

                var workerEntity = SpawnWorker(ecb, spawner.FullBucketWorkerPrefab, GenFieldPos(random, radius));
                ecb.SetComponent(fetcherEntity, new DestinationWorker() {Value = workerEntity});
                ecb.AppendToBuffer<Member>(captainEntity, workerEntity);

                var previousMember = workerEntity;
                
                for (var j = 1; j < squadSize; j++)
                {
                    workerEntity = SpawnWorker(ecb, spawner.FullBucketWorkerPrefab, GenFieldPos(random, radius));
                    ecb.SetComponent(previousMember, new DestinationWorker() {Value = workerEntity});
                    
                    ecb.AppendToBuffer<Member>(captainEntity, workerEntity);

                    previousMember = workerEntity;
                }
                
                ecb.SetComponent(previousMember, new DestinationWorker() {Value = captainEntity});

                previousMember = captainEntity;
                
                for (var j = 0; j < squadSize; j++)
                {
                    workerEntity = SpawnWorker(ecb, spawner.EmptyBucketWorkerPrefab, GenFieldPos(random, radius));
                    ecb.SetName(workerEntity, "SquadWorker");

                    ecb.SetComponent(previousMember, new DestinationWorker() {Value = workerEntity});
                    
                    ecb.AppendToBuffer<Member>(captainEntity, workerEntity);
                    
                    previousMember = workerEntity;
                }

                ecb.AddComponent(captainEntity, new TeamInfo() { Captain = captainEntity, Fetcher = fetcherEntity });
            }
   }

   static void SpawnOmniworkers(EntityCommandBuffer ecb, Entity prefab, int count, int size, Random random)
   {
       var radius = (size - 1) / 2f;

       for (int i = 0; i < count; i++)
       {
           var entity = ecb.Instantiate(prefab);
           ecb.SetComponent(entity, new Position {Value = new float2(random.NextFloat(-radius, radius), random.NextFloat(-radius, radius))});
           ecb.SetComponent(entity, new Speed() { Value = FreeSpeed });
       }
   }

   static void SpawnBuckets(EntityCommandBuffer ecb, Entity prefab, int count, int size, Random random)
   {
       var radius = (size - 1) / 2f;

       for (int i = 0; i < count; i++)
       {
           var entity = ecb.Instantiate(prefab);
           ecb.SetComponent(entity, new Position {Value = new float2(random.NextFloat(-radius, radius), random.NextFloat(-radius, radius))});
           ecb.SetComponent(entity, new Scale { Value = new float3(EmptyWaterSize, EmptyWaterSize, EmptyWaterSize) });
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
           ecb.SetComponent(entity, new Volume() { Value = random.NextInt(minWater, maxWater)});
       }
                
       for (int i = 0; i < waterCountEachSide; i++)
       {
           var entity = ecb.Instantiate(prefab);
           ecb.SetComponent(entity, new Position {Value = new float2(radius + distanceFromEdge, -radius + waterDistance * i)});
           ecb.SetComponent(entity, new Volume() { Value = random.NextInt(minWater, maxWater)});
       }
                
       for (int i = 0; i < waterCountEachSide; i++)
       {
           var entity = ecb.Instantiate(prefab);
           ecb.SetComponent(entity, new Position {Value = new float2(-radius + waterDistance * i, -radius - distanceFromEdge)});
           ecb.SetComponent(entity, new Volume() { Value = random.NextInt(minWater, maxWater)});
       }
                
       for (int i = 0; i < waterCountEachSide; i++)
       {
           var entity = ecb.Instantiate(prefab);
           ecb.SetComponent(entity, new Position {Value = new float2(-radius + waterDistance * i, radius + distanceFromEdge)});
           ecb.SetComponent(entity, new Volume() { Value = random.NextInt(minWater, maxWater)});
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
            .WithName("SpawnerSystem")
            .ForEach((Entity entity, in Spawner spawner) =>
            {
                
                ecb.DestroyEntity(entity);

                SpawnCommunicationBuffers(ecb);
                
                SpawnHeatmap(ecb, spawner.FireDimension, spawner.firePropagationSpeed);

                SpawnSplashmap(ecb, spawner.BucketCount);

                if (spawner.FlameCellPrefab != Entity.Null)
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
