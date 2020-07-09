using Unity.Entities;
using Unity.Mathematics;

namespace AutoFarmers
{
    [UpdateAfter(typeof(SimulationSystemGroup))]
    public class TillField_TillCell : SystemBase
    {
        protected override void OnCreate()
        {
            GetEntityQuery(ComponentType.ReadWrite<CellTypeElement>());
        }
        
        protected override void OnUpdate()
        {
            Entity grid = GetSingletonEntity<Grid>();
            Grid gridComponent = EntityManager.GetComponentData<Grid>(grid);
            int2 gridSize = gridComponent.Size;
            
            ComponentDataFromEntity<CellPosition> cellPositionAccessor = GetComponentDataFromEntity<CellPosition>();
            DynamicBuffer<CellTypeElement> cellTypeBuffer = EntityManager.GetBuffer<CellTypeElement>(grid);
            DynamicBuffer<CellEntityElement> cellEntityBuffer = EntityManager.GetBuffer<CellEntityElement>(grid);
            
            Entities
                .WithAll<TillField_Intent>()
                .WithAll<TargetReached>()
                .WithStructuralChanges()
                .ForEach((Entity entity, in Target target) =>
                {
                    CellPosition cp = cellPositionAccessor[target.Value];
                    int index = (int) (cp.Value.x * gridSize.x + cp.Value.y);
                    
                    cellTypeBuffer[index] = new CellTypeElement() { Value = CellType.Tilled };
                    EntityManager.AddComponent<Tilled>(cellEntityBuffer[index].Value);
                    
                    EntityManager.AddComponent<Cooldown>(entity);
                    EntityManager.SetComponentData<Cooldown>(entity, new Cooldown() { Value = 0.1f});
                    EntityManager.RemoveComponent<Target>(entity);
                    EntityManager.RemoveComponent<TargetReached>(entity);
                }).Run();
        }
    }
}
