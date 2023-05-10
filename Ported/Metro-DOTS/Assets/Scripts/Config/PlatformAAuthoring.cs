using Components;
using UnityEngine;
using Unity.Entities;

namespace Metro
{
    public class PlatformAAuthoring : MonoBehaviour
    {
        class Baker : Baker<PlatformAAuthoring>
        {
            public override void Bake(PlatformAAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.Dynamic);
                AddComponent<PlatformA>(entity);
            }
        }
    }
}
