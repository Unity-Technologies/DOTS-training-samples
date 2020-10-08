using Unity.Collections;
using Unity.Entities;

public class CarriageCreationSystem : SystemBase
{
    protected override void OnUpdate()
    {
        var carriagePrefab = GetSingleton<MetroBuilder>().CarriagePrefab;
        
        Entities.WithStructuralChanges()
            .WithAll<BufferCarriage>()
            .ForEach((Entity entity, in Rail rail, in CarriageCount carriageCount, in Position position) =>
            {
                var carriages = EntityManager.Instantiate(carriagePrefab, carriageCount, Allocator.Temp);
                
                var carriageBuffer = EntityManager.GetBuffer<BufferCarriage>(entity);
                for (int i = 0; i < carriageCount; i++)
                {
                    var carriage = carriages[i];
                    EntityManager.SetComponentData(carriage, rail);
                    carriageBuffer.Add(carriage);
                }

                carriages.Dispose();

                EntityManager.RemoveComponent<CarriageCount>(entity);
            }).Run();
    }
}
