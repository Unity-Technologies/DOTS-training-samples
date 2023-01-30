using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

public class QueueAuthoring : MonoBehaviour
{
    public float3 QueueDirection;
    public int QueueCapacity;

    class Baker : Baker<QueueAuthoring>
    {
        public override void Bake(QueueAuthoring authoring)
        {
            AddComponent(new Queue()
            {
                QueueDirection = authoring.QueueDirection,
                QueueCapacity = authoring.QueueCapacity
            });
        }
    }
}

struct Queue : IComponentData
{
    public float3 QueueDirection;
    public int QueueSize;
    public int QueueCapacity;
}
