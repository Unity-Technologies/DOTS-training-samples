using Unity.Entities;
using UnityEngine.PlayerLoop;

namespace AutoFarmers
{
    [UpdateInGroup(typeof(SimulationSystemGroup), OrderFirst = true)]
    [UpdateAfter(typeof(BeginSimulationEntityCommandBufferSystem))]
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
                var i = grid.GetIndexFromCoords(cellPosition.Value);
                typeBuffer[i] = new CellTypeElement(cell.Type);
            }).ScheduleParallel(Dependency).Complete();
        }
    }
}