using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;

[UpdateInGroup(typeof(InitializationSystemGroup))]
public class ProjectileInitSpawnerSystem: JobComponentSystem
{
    private BeginInitializationEntityCommandBufferSystem m_spawnerECB;
    
    protected override void OnCreate()
    {
        m_spawnerECB = World.GetOrCreateSystem<BeginInitializationEntityCommandBufferSystem>();
        RequireSingletonForUpdate<ProjectileSpawnerComponentData>();
    }
    
    
    protected override JobHandle OnUpdate(JobHandle inputDeps)
    {
        var ecb = m_spawnerECB.CreateCommandBuffer();
        
        Entities
            //.WithBurst(FloatMode.Default,FloatPrecision.Standard,true)
            .WithoutBurst()
            .ForEach((Entity entity, int entityInQueryIndex, ProjectileSpawnerComponentData spawner) =>
        {
            //spawn buckets for more efficient search
            float sectionLen = (spawner.xRange.y - spawner.xRange.x) / (float) spawner.numBuckets;
            
            //to help initial placement
            var sortBucket = new DynamicBuffer<ProjectileBufferData>[spawner.numBuckets];
            
            for (int i = 0; i < spawner.numBuckets; i++)
            {
                Entity bucket = ecb.CreateEntity();

                BucketComponentData data = new BucketComponentData()
                {
                    minRange = spawner.xRange.x + sectionLen * i,
                    maxRange = spawner.xRange.x + sectionLen * (i + 1),
                };

                ecb.AddComponent(bucket,data);
                var buffer = ecb.AddBuffer<ProjectileBufferData>(bucket);
                
                sortBucket[i] = buffer;

            }
            
            Random rng = new Random();
            rng.InitState();
            
            for (int i = 0; i < spawner.initSpawn; i++)
            {
                var radius = rng.NextFloat(spawner.radiusRange.x,spawner.radiusRange.y);
                var spawnX = rng.NextFloat(spawner.xRange.x, spawner.xRange.y);
                
                Entity projectile = ecb.Instantiate(spawner.prefab);
                ecb.AddComponent(projectile, new Scale
                {
                    Value = radius * 2
                });
                ecb.SetComponent(projectile, new Translation()
                {
                    Value = new float3(spawnX,0,spawner.spawnZ)
                });
                
                var data = new ProjectileComponentData()
                {
                    radius = radius,
                    velocityX = spawner.velocityX,
                    rangeXMin = spawner.xRange.x,
                    rangeXMax = spawner.xRange.y
                };
                
                ecb.AddComponent(projectile,data);
                
                //find bucketIndex based on random range
                int bucketIndex = 0;
                float range = spawner.xRange.x;
                while (range + sectionLen < spawnX)
                {
                    range += sectionLen;
                    bucketIndex++;
                }

                sortBucket[bucketIndex].Add(data);
            }

            
            
            Enabled = false;
            
        }).Run();
        
        //m_spawnerECB.AddJobHandleForProducer(spawnJobHandle);
        
        return inputDeps;
    }
}
