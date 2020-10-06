using Unity.Entities;
using Unity.Mathematics;

public class SpawningSystem : SystemBase
{
    protected override void OnUpdate()
    {
        var deltaTime = Time.DeltaTime;
        Entities.WithStructuralChanges().ForEach((ref SpawnerInfo spawnerInfo, in BoardInfo boardInfo) =>
        {
            spawnerInfo.catTimer += deltaTime;
            if (spawnerInfo.catTimer >= spawnerInfo.catFrequency)
            {
                spawnerInfo.catTimer -= spawnerInfo.catFrequency;
                var cat = EntityManager.Instantiate(spawnerInfo.catPrefab);
                EntityManager.SetComponentData(cat, new Position()
                {
                    position = new float2(1,0)
                });
            }
        }).Run();
    }
}
