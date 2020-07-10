using Unity.Entities;

namespace AutoFarmers
{
    [UpdateAfter(typeof(SimulationSystemGroup))]
    [UpdateBefore(typeof(PresentationSystemGroup))]
    class CellTypeSystem : SystemBase
    {
        protected override void OnUpdate()
        {
            var gridEntity = GetSingletonEntity<Grid>();
            var grid = GetComponent<Grid>(gridEntity);
            var typeBuffer = EntityManager.GetBuffer<CellTypeElement>(gridEntity);
            
            Entities.WithChangeFilter<Cell>().WithNativeDisableParallelForRestriction(typeBuffer)
                .ForEach((Entity entity, CellPosition cellPosition, Cell cell) =>
            {
                var i = cellPosition.Value.y * grid.Size.x + cellPosition.Value.x;
                typeBuffer[i] = new CellTypeElement(cell.Type);
            }).ScheduleParallel(Dependency).Complete();
        }
    }
}