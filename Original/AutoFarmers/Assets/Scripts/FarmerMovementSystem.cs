using Unity.Entities;
using Unity.Transforms;
using Unity.Rendering;

public class FarmerMovementSystem:SystemBase
{
    protected override void OnUpdate()
    {
        Entities
        .WithStructuralChanges()
        .ForEach((Entity entity, ref Position2D position, in WorkerTeleport teleportData) =>
        {
            EntityManager.SetComponentData<Position2D>(entity, new Position2D { position = teleportData.position });

           EntityManager.RemoveComponent<WorkerTeleport>(entity);

        }).Run();
    }
}
