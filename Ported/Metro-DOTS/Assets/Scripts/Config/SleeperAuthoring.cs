using UnityEngine;
using Unity.Entities;

public class SleeperAuthoring : MonoBehaviour
{
    class Baker : Baker<SleeperAuthoring>
    {
        public override void Bake(SleeperAuthoring authoring)
        {
            var entity = GetEntity(TransformUsageFlags.Renderable);
            AddComponent(entity, new SleeperTag { });
        }
    }
}