using Metro;
using UnityEngine;
using Unity.Entities;

namespace Metro
{
    public class PlatformBAuthoring : MonoBehaviour
    {
        class Baker : Baker<PlatformBAuthoring>
        {
            public override void Bake(PlatformBAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.Dynamic);
                AddComponent<PlatformB>(entity);
            }
        }
    }
}
