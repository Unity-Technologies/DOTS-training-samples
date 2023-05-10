using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

public class GridAuthoring : MonoBehaviour
{
    public int GridSize;
    public float MinGridY;
    public float GridCellSize;
    public int NumStartingFires;
    public int NumOmnibot;

    public GameObject BotPrefab;
    public GameObject FirePrefab;
    public GameObject WaterPrefab;
    public GameObject OmnibotPrefab;
    public Transform originGrid;

    class Baker : Baker<GridAuthoring>
    {
        public override void Bake(GridAuthoring authoring)
        {
            var entity = GetEntity(TransformUsageFlags.None);

            // Each authoring field corresponds to a component field of the same name.
            AddComponent(entity, new Grid
            {
                GridSize = authoring.GridSize,
                NumStartingFires = authoring.NumStartingFires,
                MinGridY = authoring.MinGridY,
                BotPrefab = GetEntity(authoring.BotPrefab, TransformUsageFlags.Dynamic),
                FirePrefab = GetEntity(authoring.FirePrefab,TransformUsageFlags.Dynamic | TransformUsageFlags.NonUniformScale),
                WaterPrefab = GetEntity(authoring.WaterPrefab,TransformUsageFlags.Dynamic | TransformUsageFlags.NonUniformScale),
                OmnibotPrefab = GetEntity(authoring.OmnibotPrefab, TransformUsageFlags.Dynamic),
                GridOrigin = new float3(authoring.originGrid.position),
                NumOmnibot = authoring.NumOmnibot
            });
        }
    }
}

public struct Grid : IComponentData
{
    public int GridSize;
    public float MinGridY;
    public float GridCellSize;
    public float PlayerOffset;
    public float PlayerSpeed;
    public int NumStartingFires;
    public Entity BotPrefab;
    public Entity FirePrefab;
    public Entity WaterPrefab;
    public float3 GridOrigin;
    public Entity OmnibotPrefab;
    public int NumOmnibot;
}
