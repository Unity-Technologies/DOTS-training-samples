using Unity.Entities;
using UnityEngine;

public class GridAuthoring : MonoBehaviour
{

    public int NumRows;
    public int NumColumns;
    public float GridCellSize;

    public GameObject BotPrefab;
    public GameObject FirePrefab;
    public GameObject WaterPrefab;

    class Baker : Baker<GridAuthoring>
    {
        public override void Bake(GridAuthoring authoring)
        {
            var entity = GetEntity(TransformUsageFlags.None);

            // Each authoring field corresponds to a component field of the same name.
            AddComponent(entity, new Grid
            {
                NumRows = authoring.NumRows,
                NumColumns = authoring.NumColumns,
                BotPrefab = GetEntity(authoring.BotPrefab, TransformUsageFlags.Dynamic),
                FirePrefab = GetEntity(authoring.FirePrefab,TransformUsageFlags.Dynamic | TransformUsageFlags.NonUniformScale),
                WaterPrefab = GetEntity(authoring.WaterPrefab,TransformUsageFlags.Dynamic | TransformUsageFlags.NonUniformScale),
            });
        }
    }
}

public struct Grid : IComponentData
{
    public int NumRows; 
    public int NumColumns;
    public float GridCellSize;
    public float PlayerOffset;
    public float PlayerSpeed;
    public Entity BotPrefab;
    public Entity FirePrefab;
    public Entity WaterPrefab;
}
