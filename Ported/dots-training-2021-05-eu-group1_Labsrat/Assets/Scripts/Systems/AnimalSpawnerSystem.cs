using Unity.Collections;
using Unity.Entities;
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

[AlwaysUpdateSystem]
public class AnimalSpawnerSystem : SystemBase
{
    bool mouseflipflop;
    float TimeUntilNextMouseSpawn;
    float TimeUntilNextCatSpawn;

    EntityQuery mouseQuery;
    EntityQuery catQuery;
    Random random;
    bool randomInitialized;

    protected override void OnCreate()
    {
        mouseQuery = GetEntityQuery(ComponentType.ReadOnly<Mouse>());
        catQuery = GetEntityQuery(ComponentType.ReadOnly<Cat>());
        randomInitialized = false;
    }

    protected override void OnUpdate()
    {
        int mousecount = mouseQuery.CalculateEntityCount();
        int catcount = catQuery.CalculateEntityCount();
        
        if (TryGetSingleton(out GameConfig gameConfig))
        {
            if(!randomInitialized)
            {
                random = Random.CreateFromIndex(gameConfig.RandomSeed ? (uint)System.DateTime.Now.Ticks : gameConfig.Seed ^ 2984576396);
                randomInitialized = true;
            }

            if (catcount < gameConfig.NumOfCats)
            {

                if (TimeUntilNextCatSpawn <= 0)
                {

                    var xPos = random.NextInt(gameConfig.BoardDimensions.x);
                    var yPos = random.NextInt(gameConfig.BoardDimensions.y);
                    var randDir = random.NextInt(3);
                    var rotation = Unity.Mathematics.quaternion.RotateY(Mathf.PI * randDir / 2);

                    Entity cat = EntityManager.Instantiate(gameConfig.CatPrefab);
                    TimeUntilNextCatSpawn = gameConfig.CatSpawnDelay;
                    EntityManager.AddComponent<Cat>(cat);
                    EntityManager.AddComponent<Translation>(cat);
                    EntityManager.SetComponentData(cat, new Translation() { Value = new float3(xPos, 0, yPos) });
                    EntityManager.SetComponentData(cat, new Rotation() { Value = rotation });
                    EntityManager.AddComponent<Direction>(cat);
                    EntityManager.SetComponentData(cat, new Direction() { Value = Direction.FromRandomDirection(randDir) });

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
                    int xPos = 0;
                    int yPos = 0;
                    Cardinals dir = Cardinals.East;
                    if (mouseflipflop)
                    {
                        xPos = gameConfig.BoardDimensions.x - 1;
                        yPos = gameConfig.BoardDimensions.y - 1;
                        dir = Cardinals.West;
                    }

                    var rotation = Unity.Mathematics.quaternion.RotateY(Direction.GetAngle(dir));

                    Entity mouse = EntityManager.Instantiate(gameConfig.MousePrefab);
                    TimeUntilNextMouseSpawn = gameConfig.MouseSpawnDelay;
                    EntityManager.AddComponent<Translation>(mouse);
                    EntityManager.AddComponent<Mouse>(mouse);
                    EntityManager.AddComponent<Direction>(mouse);

                    EntityManager.SetComponentData(mouse, new Translation() { Value = new float3(xPos, 0, yPos) });
                    EntityManager.SetComponentData(mouse, new Rotation() { Value = rotation });
                    EntityManager.SetComponentData(mouse, new Direction() { Value = dir });
                    mouseflipflop = !mouseflipflop;

                }
                else
                {
                    TimeUntilNextMouseSpawn -= Time.DeltaTime;
                }
            }

            //Enabled = false;
        }
    }
}
