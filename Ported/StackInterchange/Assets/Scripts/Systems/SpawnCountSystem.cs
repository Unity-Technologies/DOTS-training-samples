using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

public class SpawnCountSystem : SystemBase
{
    protected override void OnUpdate()
    {
        // Count number of cars ???
        int count = 0;
        Entities
            .ForEach((Entity entity, in CarMovement movement) =>
            {
                count++;
            }).Run();

        Entities
            .ForEach((Entity entity, ref SpawnCount spawnCount) =>
            {
                spawnCount.TotalCount = count;
            }).Run();
    }
}
