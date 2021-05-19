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

public class AnimalSpawnerSystem : SystemBase
{
    int catcount = 0;
    int mousecount = 0;
    bool mouseflipflop = false;
    float respawn = 0f;
    protected override void OnUpdate()
    {
        
        if (TryGetSingleton(out GameConfig gameConfig))
        {
            var random = Random.CreateFromIndex(gameConfig.RandomSeed ? (uint)System.DateTime.Now.Ticks : gameConfig.Seed ^ 2984576396);

            if (catcount < gameConfig.NumOfCats)
            {
                int countToCreate = gameConfig.NumOfCats - catcount;
                for (int i = 0; i < countToCreate; i++)
                {
                    var xPos = random.NextInt(gameConfig.BoardDimensions.x);
                    var yPos = random.NextInt(gameConfig.BoardDimensions.y);
                    var randDir = random.NextInt(3);
                    var rotation = Unity.Mathematics.quaternion.RotateY(Mathf.PI * randDir / 2);

                    Entity cat = EntityManager.Instantiate(gameConfig.CatPrefab);
                    EntityManager.AddComponent<Cat>(cat);
                    EntityManager.AddComponent<Translation>(cat);
                    EntityManager.SetComponentData(cat, new Translation() { Value = new float3(xPos, 0, yPos) });
                    EntityManager.SetComponentData(cat, new Rotation() { Value = rotation });
                    EntityManager.AddComponent<Direction>(cat);
                    EntityManager.SetComponentData(cat, new Direction() { Value = Direction.FromRandomDirection(randDir) });

                    EntityManager.AddComponent<Position>(cat);
                    EntityManager.SetComponentData(cat, new Position() { Value = new float2(xPos, yPos) });
                    catcount++;
               
                }
            }

            if (mousecount < gameConfig.NumOfMice)
            {
                int countToCreate = gameConfig.NumOfMice - mousecount;
                if (respawn <= 0)
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
                    respawn = 0.2f;
                    EntityManager.AddComponent<Translation>(mouse);
                    EntityManager.AddComponent<Mouse>(mouse);
                    EntityManager.AddComponent<Direction>(mouse);
                    EntityManager.AddComponent<Position>(mouse);

                    EntityManager.SetComponentData(mouse, new Translation() { Value = new float3(xPos, 0, yPos) });
                    EntityManager.SetComponentData(mouse, new Rotation() { Value = rotation });
                    EntityManager.SetComponentData(mouse, new Direction() { Value = dir });
                    EntityManager.SetComponentData(mouse, new Position() { Value = new float2(xPos, yPos) });
                    mousecount++;
                    mouseflipflop = !mouseflipflop;

                }
                else
                {
                    respawn -= Time.DeltaTime;
                }
            }

            //Enabled = false;
        }
    }
}
