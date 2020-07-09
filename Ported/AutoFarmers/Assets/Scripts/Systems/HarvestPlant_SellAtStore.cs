using Unity.Entities;
using Unity.Mathematics;

namespace AutoFarmers
{
    [UpdateAfter(typeof(SimulationSystemGroup))]
    public class HarvesPlant_SellAtStore : SystemBase
    {
        private EntityCommandBufferSystem _entityCommandBufferSystem;

        protected override void OnCreate()
        {
            _entityCommandBufferSystem = World.GetExistingSystem<EndSimulationEntityCommandBufferSystem>();
            GetEntityQuery(ComponentType.ReadWrite<CellTypeElement>());
        }
        
        protected override void OnUpdate()
        {
            EntityCommandBuffer ecb = _entityCommandBufferSystem.CreateCommandBuffer();

            Entity grid = GetSingletonEntity<Grid>();
            Grid gridComponent = EntityManager.GetComponentData<Grid>(grid);
            int2 gridSize = gridComponent.Size;
            
            ComponentDataFromEntity<DepositedPlantCount> plantCoutnAcessor = GetComponentDataFromEntity<DepositedPlantCount>();
            DynamicBuffer<CellTypeElement> cellTypeBuffer = EntityManager.GetBuffer<CellTypeElement>(grid);
            DynamicBuffer<CellEntityElement> cellEntityBuffer = EntityManager.GetBuffer<CellEntityElement>(grid);
            
            Entities
                .WithAll<HarvestPlant_Intent>()
                .WithAll<TargetReached>()
                .WithNone<HarvestPlant>()
                .ForEach((Entity entity, ref PathFindingTarget target, in Home home) =>
                {                    
                    ecb.RemoveComponent<TargetReached>(entity);
                    ecb.RemoveComponent<HarvestPlant_Intent>(entity);
                    DepositedPlantCount cnt = plantCoutnAcessor[home.Value];
                    cnt.ForDrones++;
                    cnt.ForFarmers++;
                    ecb.SetComponent(home.Value, cnt);
                    ecb.RemoveComponent<Target>(entity);
                    ecb.RemoveComponent<PathFindingTarget>(entity);
                    ecb.RemoveComponent<TakePlantToStore>(entity);
                    UnityEngine.Debug.Log("Sold");
                }).Schedule();

            _entityCommandBufferSystem.AddJobHandleForProducer(Dependency);
        }
    }
}
