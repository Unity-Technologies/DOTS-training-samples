using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

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
            
            ComponentDataFromEntity<LocalToWorld> localToWorldAccessor = GetComponentDataFromEntity<LocalToWorld>();
            DynamicBuffer<CellTypeElement> cellTypeBuffer = EntityManager.GetBuffer<CellTypeElement>(grid);
            DynamicBuffer<CellEntityElement> cellEntityBuffer = EntityManager.GetBuffer<CellEntityElement>(grid);
            
            Entities
                .WithAll<TillField_Intent>()
                .WithAll<TargetReached>()
                .WithStructuralChanges()
                .ForEach((Entity entity, in Target target) =>
                {
                    LocalToWorld l2w = localToWorldAccessor[target.Value];
                    int index = (int) (l2w.Position.x * gridSize.x + l2w.Position.z);
                    
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
