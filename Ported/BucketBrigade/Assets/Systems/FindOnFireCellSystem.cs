using Unity.Collections;
using Unity.Entities;
using Unity.Transforms;

public class FindOnFireCellSystem : SystemBase
{
    public static EntityQuery onFireCells;

    protected override void OnCreate()
    {
        base.OnCreate();
        onFireCells = GetEntityQuery(ComponentType.ReadOnly<FireCell>(), ComponentType.ReadOnly<OnFire>(), ComponentType.ReadOnly<Translation>());

    }
    protected override void OnUpdate()
    {
        var ecb = new EntityCommandBuffer(Allocator.TempJob);

        var onFireCellsEntities = onFireCells.ToEntityArray(Allocator.TempJob);
        var onFireCellsTranslations = onFireCells.ToComponentDataArray<Translation>(Allocator.TempJob);

        Entities.ForEach((Entity entity, in FindOnFireCell bot, in Translation translation) =>
        {
            ecb.RemoveComponent<FindOnFireCell>(entity);

            Entity closestOnFireCell = FireSim.GetClosestEntity(translation.Value, onFireCellsEntities, onFireCellsTranslations);

            ecb.AddComponent(entity, new MoveTowardFire { Target = closestOnFireCell });
        }).Run();

        ecb.Playback(World.EntityManager);
        ecb.Dispose();

        onFireCellsEntities.Dispose();
        onFireCellsTranslations.Dispose();
    }
}
