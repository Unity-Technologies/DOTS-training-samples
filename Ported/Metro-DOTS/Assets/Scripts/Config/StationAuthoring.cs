using Components;
using UnityEngine;
using Unity.Entities;

namespace Metro
{
    public class StationAuthoring : MonoBehaviour
    {
        class Baker : Baker<StationAuthoring>
        {
            public override void Bake(StationAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.Dynamic);
                AddComponent<StationIDComponent>(entity);
            }
        }
    }
}
