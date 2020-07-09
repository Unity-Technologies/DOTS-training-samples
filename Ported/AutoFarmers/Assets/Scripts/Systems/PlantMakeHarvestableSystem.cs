using AutoFarmers;
using Unity.Entities;
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
        DynamicBuffer<CellTypeElement> typeBuffer = EntityManager.GetBuffer<CellTypeElement>(grid);
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
            .ForEach((int entityInQueryIndex, Entity entity, ref Translation translation) =>
        {            
            ecb.AddComponent<HarvestablePlant_Tag>(entity, new HarvestablePlant_Tag());
               
            // Update the cell type
            {
                int gridX = (int)(translation.Value.x);
                int gridY = (int)(translation.Value.z);

                int index = gridX * gridSize.x + gridY;
                if (index < 0 || index >= typeBuffer.Length)
                {
                    UnityEngine.Debug.Log("Out of bounds index in PlantMakeHarvestableSystem!");
                }
                else
                {
                    typeBuffer[index] = new CellTypeElement() { Value = CellType.Plant };
                }
            }
        }).Run(); // Run for now until we can get this working 100% in parallel

        m_CommandBufferSystem.AddJobHandleForProducer(Dependency);
    }
}

