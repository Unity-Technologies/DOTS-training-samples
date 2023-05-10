using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

public class GridAuthoring : MonoBehaviour
{
    public int GridSize;
    public float GridCellSize;

    public GameObject BotPrefab;
    public GameObject FirePrefab;
    public GameObject WaterPrefab;
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
                BotPrefab = GetEntity(authoring.BotPrefab, TransformUsageFlags.Dynamic),
                FirePrefab = GetEntity(authoring.FirePrefab,TransformUsageFlags.Dynamic | TransformUsageFlags.NonUniformScale),
                WaterPrefab = GetEntity(authoring.WaterPrefab,TransformUsageFlags.Dynamic | TransformUsageFlags.NonUniformScale),
                GridOrigin = new float3(authoring.originGrid.position)
            });
        }
    }
}

public struct Grid : IComponentData
{
    public int GridSize; 
    public float GridCellSize;
    public float PlayerOffset;
    public float PlayerSpeed;
    public Entity BotPrefab;
    public Entity FirePrefab;
    public Entity WaterPrefab;
    public float3 GridOrigin;
}
