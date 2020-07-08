using AutoFarmers;
using Unity.Entities;
using Unity.Mathematics;

public class PlantMakeHarvestableSystem : SystemBase
{
    private EntityCommandBufferSystem m_CommandBufferSystem;

    protected override void OnCreate()
    {
        m_CommandBufferSystem = World.GetExistingSystem<EndInitializationEntityCommandBufferSystem>();
    }

    protected override void OnUpdate()
    {
        var ecb = m_CommandBufferSystem.CreateCommandBuffer().ToConcurrent();

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
        
        Entities
            .WithAll<FullyGrownPlant_Tag>()            
            .WithNone<HarvestablePlant_Tag>()
            .ForEach((int entityInQueryIndex, Entity entity, ref Position position) =>
        {            
            ecb.AddComponent<HarvestablePlant_Tag>(entityInQueryIndex, entity, new HarvestablePlant_Tag());
               
            // Update the cell type
            {
                int index = (int)position.Value.y / gridSize.x + (int)position.Value.x;
                if (index < 0 || index >= typeBuffer.Length)
                {
                    UnityEngine.Debug.Log("Out of bounds index in PlantMakeHarvestableSystem!");
                }
                else
                {
                    typeBuffer[index] = new CellTypeElement() { Value = CellType.Plant };
                }
            }
        }).Schedule();

        m_CommandBufferSystem.AddJobHandleForProducer(Dependency);
    }
}

