using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

public class InitWorldStateSystem : SystemBase
{
    public int GridWidth;
    public int GridHeight;

    public Entity FirePrefab;

    private bool m_Initialized;

    protected override void OnUpdate()
    {
        if (!m_Initialized)
        {
            m_Initialized = true;

            var grid = GridUtils.CreateGrid(GridWidth, GridHeight);
            grid.Heat[grid.GetIndex((GridWidth / 2, GridHeight / 2))] = 0.1f; // TEMP: Place a cell on fire

            // Spawn grid cells
            for (int i = 0; i < GridWidth * GridHeight; i++)
            {
                var entity = EntityManager.Instantiate(FirePrefab);
                var comp = new GridCell { Index = i };
                EntityManager.AddComponentData(entity, comp);
                // TODO: Translation component should not be needed to build LocalToWorld
                EntityManager.SetComponentData(entity, new Translation{ Value = new float3(i % GridWidth, 0, i / GridWidth) });
            }
        }
    }
}
