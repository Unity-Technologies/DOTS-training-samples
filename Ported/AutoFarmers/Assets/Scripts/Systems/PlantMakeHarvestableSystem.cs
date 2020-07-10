using AutoFarmers;
using Unity.Entities;
using Unity.Jobs.LowLevel.Unsafe;
using Unity.Mathematics;
using Unity.Transforms;

public class PlantMakeHarvestableSystem : SystemBase
{
    private EntityCommandBufferSystem m_CommandBufferSystem;

    protected override void OnCreate()
    {
        m_CommandBufferSystem = World.GetExistingSystem<EndInitializationEntityCommandBufferSystem>();
        GetEntityQuery(ComponentType.ReadWrite<CellTypeElement>());
    }

    protected override void OnUpdate()
    {
        Entity grid = GetSingletonEntity<Grid>();
        Grid gridComponent = EntityManager.GetComponentData<Grid>(grid);
        var entityBuffer = EntityManager.GetBuffer<CellEntityElement>(grid);
        int2 gridSize = gridComponent.Size;
        if (gridSize.x == 0 || gridSize.y == 0)
        {
            // You don't have a grid initialized yet!
            UnityEngine.Debug.Log("Not running PlantMakeharvestableSystem, you don't have a grid sized > 0!");
            return; 
        }

        var ecb = m_CommandBufferSystem.CreateCommandBuffer();

        Entities
            .WithAll<FullyGrownPlant_Tag>()            
            .WithNone<HarvestablePlant_Tag>()
            .WithReadOnly(entityBuffer)
            .ForEach((int entityInQueryIndex, Entity entity, in CellPosition cp) =>
        {            
            ecb.AddComponent<HarvestablePlant_Tag>(entity);
               
            // Update the cell type
            int index = (int)(cp.Value.y * gridSize.x + cp.Value.x);
            var cellEntity = entityBuffer[index].Value;
            ecb.SetComponent(cellEntity, new Cell { Type = CellType.HarvestablePlant });
            ecb.SetComponent(cellEntity, new CellOccupant { Value = entity});

        }).Run(); // Run for now until we can get this working 100% in parallel

        m_CommandBufferSystem.AddJobHandleForProducer(Dependency);
    }
}

