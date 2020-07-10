using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

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
                .ForEach((Entity entity, ref PathFindingTarget target, ref TakePlantToStore takeToStore, in Home home) =>
                {
                    // Increase the amount in the store
                    DepositedPlantCount cnt = plantCoutnAcessor[home.Value];
                    cnt.ForDrones++;
                    cnt.ForFarmers++;
                    ecb.SetComponent(home.Value, cnt);

                    // Delete the plant entity
                    //ecb.DestroyEntity(takeToStore.Value);
                    ecb.SetComponent(takeToStore.Value, new NonUniformScale { Value = new float3(0.0001f, 0.0001f, 0.0001f) });

                    // Reset the farmer to do something new
                    ecb.RemoveComponent<TargetReached>(entity);
                    ecb.RemoveComponent<HarvestPlant_Intent>(entity);
                    ecb.RemoveComponent<Target>(entity);
                    ecb.RemoveComponent<PathFindingTarget>(entity);
                    ecb.RemoveComponent<TakePlantToStore>(entity);
                    UnityEngine.Debug.Log("Sold");
                }).Schedule();

            _entityCommandBufferSystem.AddJobHandleForProducer(Dependency);
        }
    }
}
