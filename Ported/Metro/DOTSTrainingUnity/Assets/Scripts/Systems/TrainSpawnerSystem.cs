using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;
using Unity.Transforms;

[UpdateInGroup(typeof(InitializationSystemGroup))]
public class TrainSpawnerSystem : SystemBase
{
    // entityQuery is initialized through source generation
    private EntityQuery entityQuery;

    protected override void OnUpdate()
    {
        int numTrains = entityQuery.CalculateEntityCount();
        Entities.WithStoreEntityQueryInField(ref entityQuery)
            .ForEach((int entityInQueryIndex, ref TrainCurrDistance currDists, in TrainTotalDistance totalDists) =>
        {
            currDists.value = totalDists.value / numTrains * ((entityInQueryIndex + 1) % numTrains);
        }).Run();
        Enabled = false;
    }
}
