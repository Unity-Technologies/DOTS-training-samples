using Unity.Entities;
using Unity.Mathematics;

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
        Entities.WithStructuralChanges().ForEach((ref SpawnerInfo spawnerInfo) =>
        {
            spawnerInfo.catTimer += deltaTime;
            if (spawnerInfo.catTimer >= spawnerInfo.catFrequency && catQuery.CalculateEntityCount() < spawnerInfo.catMax)
            {
                spawnerInfo.catTimer -= spawnerInfo.catFrequency;
                var cat = EntityManager.Instantiate(spawnerInfo.catPrefab);
                EntityManager.SetComponentData(cat, new Position()
                {
                    position = new float2(0,0)
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
            if (spawnerInfo.mouseTimer >= spawnerInfo.mouseFrequency && mouseQuery.CalculateEntityCount() < spawnerInfo.maxMiceCount)
            {
                spawnerInfo.mouseTimer -= spawnerInfo.mouseFrequency;
                var mouse = EntityManager.Instantiate(spawnerInfo.mousePrefab);
                EntityManager.SetComponentData(mouse, new Position()
                {
                    position = new float2(0,0)
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
