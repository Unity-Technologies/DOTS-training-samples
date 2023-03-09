using Unity.Entities;
using UnityEngine;

public class FlameCellAuthoring : MonoBehaviour
{
    class Baker : Baker<FlameCellAuthoring>
    {
        public override void Bake(FlameCellAuthoring cellAuthoring)
        {
            AddComponent<FlameCell>();
        }
    }
}

public struct FlameCell : IComponentData
{
    public bool isOnFire;
    public int heatMapIndex;
}