using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
using Random = Unity.Mathematics.Random;

public class SpawningSystem : SystemBase
{
    private EntityQuery mouseQuery;
    private EntityQuery catQuery;
    private Random randomNumber;
    protected override void OnCreate()
    {
        mouseQuery = EntityManager.CreateEntityQuery(typeof(Mouse));
        catQuery = EntityManager.CreateEntityQuery(typeof(Cat));
        randomNumber = new Random(1);
    }

    protected override void OnUpdate()
    {
        var deltaTime = Time.DeltaTime;
        //var boardInfo = GetSingleton<BoardInfo>();
        Entities.WithStructuralChanges().ForEach((ref SpawnerInfo spawnerInfo, in Translation translation) =>
        {
            spawnerInfo.catTimer += deltaTime;
            if (spawnerInfo.catTimer >= spawnerInfo.catFrequency && spawnerInfo.catCount < spawnerInfo.catMax)
            {
                spawnerInfo.catTimer -= spawnerInfo.catFrequency;
                var cat = EntityManager.Instantiate(spawnerInfo.catPrefab);
                spawnerInfo.catCount++;
                Debug.Log("Setting to position: " + translation.Value);

                EntityManager.SetComponentData(cat, new Translation()
                {
                    Value = new float3(translation.Value.x,0, translation.Value.z)
                });
                EntityManager.AddComponentData(cat, new Cat()
                {
                    Speed = spawnerInfo.catSpeed
                });
                EntityManager.SetComponentData(cat, new EntitySpeed()
                {
                    speed = randomNumber.NextInt(spawnerInfo.minCatSpeed, spawnerInfo.maxCatSpeed)
                });
            }

            spawnerInfo.mouseTimer += deltaTime;
            if (spawnerInfo.mouseTimer >= spawnerInfo.mouseFrequency && spawnerInfo.mouseCount < spawnerInfo.maxMiceCount)
            {
                spawnerInfo.mouseTimer -= spawnerInfo.mouseFrequency;
                var mouse = EntityManager.Instantiate(spawnerInfo.mousePrefab);
                spawnerInfo.mouseCount++;
                EntityManager.SetComponentData(mouse, new Translation()
                {
                    Value = new float3(translation.Value.x,0, translation.Value.z)
                });
                EntityManager.AddComponentData(mouse, new Mouse()
                {
                    Speed = spawnerInfo.mouseSpeed
                });
                EntityManager.SetComponentData(mouse, new EntitySpeed()
                {
                    speed = randomNumber.NextInt(spawnerInfo.minMouseSpeed, spawnerInfo.maxMouseSpeed)
                });
            }
        }).Run();
    }
}
