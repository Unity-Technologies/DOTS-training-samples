using Unity.Entities;
using Unity.Collections;

public class PheromoneMapInitSystem : SystemBase
{
    protected override void OnUpdate()
    {
        Entities.WithStructuralChanges().ForEach((Entity entity, in PheromoneMap map, in PheromoneInit tag) => {
            var buffer = EntityManager.AddBuffer<PheromoneStrength>(entity);
            var storage = new NativeArray<PheromoneStrength>(map.Resolution * map.Resolution, Allocator.Temp, NativeArrayOptions.ClearMemory);
            buffer.AddRange(storage);
            storage.Dispose();
            EntityManager.RemoveComponent<PheromoneInit>(entity);
        }).Run();
    }
}
