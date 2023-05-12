using UnityEngine;
using Unity.Entities;

namespace Metro
{
    public class QueueAuthoring : MonoBehaviour
    {
        class Baker : Baker<QueueAuthoring>
        {
            public override void Bake(QueueAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.None);
                AddComponent<QueueComponent>(entity);
                AddBuffer<QueuePassengers>(entity);
            }
        }
    }
}
