using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

public partial class FoodSpawner : SystemBase
{
    protected override void OnUpdate()
    {
        var spawner = GetSingletonEntity<GlobalData>();
        var spawnerComponent = GetComponent<GlobalData>(spawner);
        var foodBox = GetComponent<AABB>(spawnerComponent.FoodPrefab);

        var foodWidth = foodBox.halfSize.x*2.0f+0.1f;
        var foodHeight = foodBox.halfSize.y*2.0f+0.1f;
        
        var columnCount = (int)((spawnerComponent.BoundsMax.z - spawnerComponent.BoundsMin.z)/foodWidth);
        var rowCount = spawnerComponent.StartingFoodCount / columnCount;

        var columnStart = new float3(
            (float)rowCount / 2.0f * foodWidth,
            spawnerComponent.BoundsMax.y - foodHeight,
            spawnerComponent.BoundsMin.z + foodWidth / 2.0f
        );
        
        var random = new Random(1234);

        // Entities
        //     .WithAll<HiveTag>()
        //     .WithStructuralChanges()
        //     .ForEach((in AABB aabb, in TeamID teamID) => 
        // {
            int foodCount = 0;

            for (int y = 0; y <= rowCount && foodCount < spawnerComponent.StartingFoodCount; y++)
            {
                var walk = columnStart;
                columnStart.x -= foodWidth;
                for (int x = 0; x < columnCount && foodCount < spawnerComponent.StartingFoodCount; x++)
                {
                    var entity = EntityManager.Instantiate(spawnerComponent.FoodPrefab);
                    EntityManager.SetComponentData(entity, new Translation {
                        Value = walk
                    });
                    walk.z += foodWidth;
                    foodCount++;
                }
            }
        // }).Run();
        this.Enabled = false;
    }
}
