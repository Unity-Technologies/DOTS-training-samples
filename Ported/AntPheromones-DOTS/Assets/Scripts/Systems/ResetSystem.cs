using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
using Random = Unity.Mathematics.Random;

public class ResetSystem : SystemBase
{
    private EndSimulationEntityCommandBufferSystem _endSimulationSystem;
    private Entity _obstacleEntity;
    private Random _random;
    
    protected override void OnCreate()
    {
        _endSimulationSystem = World.GetExistingSystem<EndSimulationEntityCommandBufferSystem>();
        _random = new Random(6541);
    }

    protected override void OnUpdate()
    {
        var random = _random;
        var center = new Translation{ Value = new float3(64, 64, 0) };
        var minRange = new float2(-1,-1);
        var maxRange = new float2(1,1);

        if (Input.GetKeyUp(KeyCode.R))
        {
            // Reset Ants
            Entities
                .WithAll<Ant>()
                .ForEach((ref Heading heading, ref Translation translation) =>
                {
                    heading.heading = math.normalize(random.NextFloat2(minRange, maxRange));
                    translation.Value = center.Value;
                }).Run();
            
            // Reset Obstacles
            // Because the number of obstacles can change, just destroy and re-create
            var ecb = _endSimulationSystem.CreateCommandBuffer();
            ecb.DestroyEntity(GetEntityQuery(typeof(Obstacle)));
            Entities
                .ForEach((in Reset reset) =>
                {
                    // Create Obstacles
                    for (int i = 1; i <= 3; i++)
                    {
                        float ringRadius = (i / (3 + 1f)) * (128 * 0.5f);
                        float circumference = ringRadius * 2f * math.PI;
                        float obstacleRadius = 1.25f;
                        int maxCount = Mathf.CeilToInt(circumference / (2f * obstacleRadius));
                        int gapAngle = random.NextInt(0, 300);
                        Debug.Log(gapAngle);
                        int gapSize = random.NextInt(30, 60);
                        for (int j = 0; j < maxCount; j++) 
                        {
                            float angle = (j) / (float)maxCount * (2f * Mathf.PI);
                            if (angle * Mathf.Rad2Deg >= gapAngle && angle * Mathf.Rad2Deg < gapAngle + gapSize)
                            {
                                continue;
                            }
                            var obstacle =  ecb.Instantiate(reset.obstaclePrefab);
                            var translation = new Translation
                            {
                                Value = new float3(64f + math.cos(angle) * ringRadius,
                                    64f + math.sin(angle) * ringRadius, 0)
                            };
                            ecb.SetComponent(obstacle, translation);
                        }
                    }
                }).Schedule();
            
            _endSimulationSystem.AddJobHandleForProducer(Dependency);

            // Reset Food
            
            // Reset Pheremones
            var pheromoneEntity = GetSingletonEntity<Pheromones>();
            var pheromoneGrid = EntityManager.GetBuffer<Pheromones>(pheromoneEntity);
        
            Dependency = Job.WithCode(() =>
                {
                    for (int i = 0; i < pheromoneGrid.Length; i++)
                    {
                        pheromoneGrid[i] = new Pheromones {pheromoneStrength = 0f};
                    }
                }
            ).Schedule(Dependency);
        }
    }
}
