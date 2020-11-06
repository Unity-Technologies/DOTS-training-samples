using Unity.Collections;
using Unity.Entities;
using Unity.Transforms;

public class FindWaterCellSystem : SystemBase
{
    public static EntityQuery waterCells;

    protected override void OnCreate()
    {
        base.OnCreate();
        waterCells = EntityManager.CreateEntityQuery(ComponentType.ReadOnly<WaterCell>(), ComponentType.ReadOnly<Translation>());

    }
    protected override void OnUpdate()
    {
        var ecb = new EntityCommandBuffer(Allocator.TempJob);

        var waterCellsEntities = waterCells.ToEntityArray(Allocator.Temp);
        var waterCellsTranslations = waterCells.ToComponentDataArray<Translation>(Allocator.Temp);

        
        Entities
            .WithReadOnly(waterCellsEntities)
            .WithReadOnly(waterCellsTranslations)
            .ForEach((Entity entity, in FindWaterCell scooperBot, in Translation translation) =>
        {
            ecb.RemoveComponent<FindWaterCell>(entity);

            Entity closestWaterCell = FireSim.GetClosestEntity(translation.Value, waterCellsEntities, waterCellsTranslations);

            ecb.AddComponent<MoveTowardWater>(entity, new MoveTowardWater { Target = closestWaterCell });
        }).Run();

        ecb.Playback(World.EntityManager);
        ecb.Dispose();

        waterCellsEntities.Dispose();
        waterCellsTranslations.Dispose();
    }
}
