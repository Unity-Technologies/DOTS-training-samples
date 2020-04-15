using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
using Random = UnityEngine.Random;

public class InitWorldStateSystem : SystemBase
{
    public int GridWidth;
    public int GridHeight;
    public int StartingFireCount;
    public int RandomSeed;

    public Entity FirePrefab;

    private bool m_Initialized;

    protected override void OnUpdate()
    {
        if (!m_Initialized)
        {
            if (RandomSeed != 0)
            {
                Random.InitState(RandomSeed);
            }

            m_Initialized = true;

            var grid = GridUtils.CreateGrid(GridWidth, GridHeight);

            // Spawn grid cells
            for (int i = 0; i < GridWidth * GridHeight; i++)
            {
                var entity = EntityManager.Instantiate(FirePrefab);
                var comp = new GridCell { Index = i };
                EntityManager.AddComponentData(entity, comp);
                // TODO: Translation component should not be needed to build LocalToWorld
                EntityManager.SetComponentData(entity, new Translation{ Value = new float3(i % GridWidth, 0, i / GridWidth) });
            }
            
            // Start random fires
            for (int i = 0; i < StartingFireCount; i++)
            {
                grid.Heat[grid.GetIndex((Random.Range(0, GridWidth), Random.Range(0, GridHeight)))] = 0.1f;
            }
        }
    }
}
