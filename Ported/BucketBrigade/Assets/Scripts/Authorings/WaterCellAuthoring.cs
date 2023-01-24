using Unity.Entities;
using UnityEngine;

public class WaterCellAuthoring : MonoBehaviour
{
    class Baker : Baker<WaterCellAuthoring>
    {
        public override void Bake(WaterCellAuthoring authoring)
        {
            AddComponent<Position>();
            AddComponent<WaterAmount>();
        }
    }
}
