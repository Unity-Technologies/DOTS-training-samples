using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;

public class GameGenSystem : SystemBase
{
    Random random;
    protected override void OnCreate()
    {
        random = new Random(42);
    }

    protected override void OnUpdate()
    {

        Entities
            .WithStructuralChanges()
            .ForEach(
            (
                Entity entity,
                in GameSpawn gameSpawn,
                in GameState gameState
            ) =>
            {
                for (int y = 0; y < gameState.GridSize.y; y++)
                {
                    for (int x = 0; x < gameState.GridSize.x; x++)
                    {
                        const float TILE_SIZE = 1f;
                        Entity tilePrefab;
                        bool isWater = (random.NextFloat() < gameState.WaterProbability);
                        tilePrefab = isWater ? gameState.WaterPrefab : gameState.PlainsPrefab;

                        var newTile = EntityManager.Instantiate(tilePrefab);
                        var pos = new float3(x * TILE_SIZE, 0f, y * TILE_SIZE);
                        EntityManager.SetComponentData(newTile, new Translation { Value = pos });

                        if (!isWater)
                        {
                            // TODO: Remove this and enable above
#if false
                            //EntityManager.AddComponent<Fertility>(newTile);
                            //EntityManager.AddComponent<FertilityMaterialOverride>(newTile);
#else
                            const int MAX_FERTILITY = 10;
                            int rndFertility = MAX_FERTILITY;// random.NextInt(0, MAX_FERTILITY + 1);
                            EntityManager.AddComponentData<Fertility>(newTile, new Fertility { Value = rndFertility });
                            //float4 col = math.lerp(new float4(1, 1, 1, 1), new float4(0.3f, 1, 0.3f, 1), (float)rndFertility / (float)MAX_FERTILITY);
                            float4 col = new float4(0.3f, 1, 0.3f, 1);
                            EntityManager.AddComponentData<FertilityMaterialOverride>(newTile, new FertilityMaterialOverride { BaseColor = col });
#endif
                        }
                    }
                }
                EntityManager.RemoveComponent<GameSpawn>(entity);
            }).Run();

    }
}
