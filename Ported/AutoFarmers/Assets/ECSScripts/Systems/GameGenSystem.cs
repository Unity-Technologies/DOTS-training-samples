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
                        Entity tilePrefab = (random.NextFloat() < gameState.WaterProbability) ?
                            gameState.WaterPrefab :
                            gameState.PlainsPrefab;
                        var newTile = EntityManager.Instantiate(tilePrefab);
                        var pos = new float3(x * TILE_SIZE, 0f, y * TILE_SIZE);
                        EntityManager.SetComponentData(newTile, new Translation { Value = pos });
                    }
                }
                EntityManager.RemoveComponent<GameSpawn>(entity);
            }).Run();

    }
}
