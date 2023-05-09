using Unity.Entities;
using UnityEngine;

public class WaterAuthoring : MonoBehaviour
{
    class Baker : Baker<WaterAuthoring>
    {
        public override void Bake(WaterAuthoring authoring)
        {
            var entity = GetEntity(TransformUsageFlags.Dynamic | TransformUsageFlags.NonUniformScale);
            AddComponent<Water>(entity);
        }
    }
}

public struct Water : IComponentData
{

}