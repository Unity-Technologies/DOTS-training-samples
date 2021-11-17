using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEditor.Rendering;
using UnityEditorInternal;

public partial class FoodSpawner : SystemBase
{
    protected override void OnUpdate()
    {
        var spawner = GetSingletonEntity<Spawner>();
        var spawnerComponent = GetComponent<Spawner>(spawner);
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
                    EntityManager.AddComponentData(entity, new Ballistic());
                    EntityManager.AddComponentData(entity, new Velocity { Value = new float3(0,0,0) });
                    EntityManager.SetComponentData(entity, new Translation {
                        Value = walk
                    });
                    walk.z += foodWidth;
                }
            }
        // }).Run();
        this.Enabled = false;
    }
}
