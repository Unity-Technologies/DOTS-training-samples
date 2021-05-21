using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Rendering;
using Unity.Transforms;
using UnityEngine;
using Random = Unity.Mathematics.Random;
using UnityCamera = UnityEngine.Camera;
using UnityGameObject = UnityEngine.GameObject;
using UnityInput = UnityEngine.Input;
using UnityKeyCode = UnityEngine.KeyCode;
using UnityMeshRenderer = UnityEngine.MeshRenderer;
using UnityMonoBehaviour = UnityEngine.MonoBehaviour;
using UnityRangeAttribute = UnityEngine.RangeAttribute;

[UpdateInGroup(typeof(ChuChuRocketUpdateGroup))]
[AlwaysUpdateSystem]
public class AnimalSpawnerSystem : SystemBase
{
    float TimeUntilNextMouseSpawn;
    float TimeUntilNextCatSpawn;

    EntityQuery mouseQuery;
    EntityQuery catQuery;
    bool randomInitialized;

    protected override void OnCreate()
    {
        mouseQuery = GetEntityQuery(ComponentType.ReadOnly<Mouse>());
        catQuery = GetEntityQuery(ComponentType.ReadOnly<Cat>());
    }

    protected override void OnUpdate()
    {
        int mousecount = mouseQuery.CalculateEntityCount();
        int catcount = catQuery.CalculateEntityCount();

        if (TryGetSingleton(out GameConfig gameConfig))
        {
            EntityCommandBuffer catEcb = new EntityCommandBuffer(Allocator.TempJob);
            EntityCommandBuffer mouseEcb = new EntityCommandBuffer(Allocator.TempJob);

            JobHandle? catJob = null;
            JobHandle? mouseJob = null;

            var timeNow = System.DateTime.Now.Ticks; 
            
            if (catcount < gameConfig.NumOfCats)
            {
                if (TimeUntilNextCatSpawn <= 0)
                {
                    TimeUntilNextCatSpawn = gameConfig.CatSpawnDelay;
                    catJob = Job.WithCode(() =>
                        {
                            Random random = Random.CreateFromIndex(gameConfig.RandomSeed ? (uint)timeNow : gameConfig.Seed ^ 2984576396);
                            for (int i = 0; i < gameConfig.MaxAnimalsSpawnedPerFrame; i++)
                            {
                                if (catcount++ >= gameConfig.NumOfCats)
                                    return;
                                
                                var xPos = random.NextInt(gameConfig.BoardDimensions.x);
                                var yPos = random.NextInt(gameConfig.BoardDimensions.y);
                                var randDir = random.NextInt(3);
                                var rotation = Unity.Mathematics.quaternion.RotateY(Mathf.PI * randDir / 2);

                                Entity cat = catEcb.Instantiate(gameConfig.CatPrefab);
                                catEcb.AddComponent<Cat>(cat);
                                catEcb.SetComponent(cat, new Translation() { Value = new float3(xPos, 0, yPos) });
                                catEcb.SetComponent(cat, new Rotation() { Value = rotation });
                                catEcb.AddComponent<Direction>(cat);
                                catEcb.SetComponent(cat, new Direction() { Value = Direction.FromRandomDirection(randDir) });
                            }
                        })
                        .WithName("SpawnCats")
                        .Schedule(Dependency);
                }
                else
                {
                    TimeUntilNextCatSpawn -= Time.DeltaTime;
                }
            }

            if (mousecount < gameConfig.NumOfMice)
            {
                if (TimeUntilNextMouseSpawn <= 0)
                {
                    TimeUntilNextMouseSpawn = gameConfig.MouseSpawnDelay;
                    
                    mouseJob = Job.WithCode(() =>
                        {
                            Random random = Random.CreateFromIndex(gameConfig.RandomSeed ? (uint)timeNow+1 : gameConfig.Seed ^ 1236534);
                            bool mouseflipflop = true;
                            quaternion angleEast = quaternion.RotateY(Direction.GetAngle(Cardinals.East));
                            quaternion angleWest = quaternion.RotateY(Direction.GetAngle(Cardinals.West));
                            for (int i = 0; i < gameConfig.MaxAnimalsSpawnedPerFrame; i++)
                            {
                                if (mousecount++ >= gameConfig.NumOfMice)
                                    return;
                                
                                int xPos = 0;
                                int yPos = 0;
                                Cardinals dir = Cardinals.East;
                                quaternion rotation = angleEast;
                                if (mouseflipflop)
                                {
                                    xPos = gameConfig.BoardDimensions.x - 1;
                                    yPos = gameConfig.BoardDimensions.y - 1;
                                    dir = Cardinals.West;
                                    rotation = angleWest;
                                }

                                if (gameConfig.MiceSpawnInRandomLocations)
                                {
                                    xPos = random.NextInt(gameConfig.BoardDimensions.x);
                                    yPos = random.NextInt(gameConfig.BoardDimensions.y);
                                }

                                Entity mouse = mouseEcb.Instantiate(gameConfig.MousePrefab);
                                mouseEcb.SetComponent(mouse, new Translation() { Value = new float3(xPos, 0, yPos) });
                                mouseEcb.SetComponent(mouse, new Rotation() { Value = rotation });
                                mouseEcb.SetComponent(mouse, new Direction() { Value = dir });
                                mouseflipflop = !mouseflipflop;
                            }
                        })
                    .WithName("SpawnMice")
                    .Schedule(Dependency);
                }
                else
                {
                    TimeUntilNextMouseSpawn -= Time.DeltaTime;
                }
            }

            if (catJob.HasValue)
            {
                catJob.Value.Complete();
                catEcb.Playback(EntityManager);
            }
            if (mouseJob.HasValue)
            {
                mouseJob.Value.Complete();
                mouseEcb.Playback(EntityManager);
            }
                
            catEcb.Dispose();
            mouseEcb.Dispose();
        }
    }
}
