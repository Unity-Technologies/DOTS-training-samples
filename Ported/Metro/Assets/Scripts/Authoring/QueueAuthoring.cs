using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

public class QueueAuthoring : MonoBehaviour
{
    public float3 QueueDirection;
    public int QueueCapacity;
    public int FacingCarriageNumber;

    class Baker : Baker<QueueAuthoring>
    {
        public override void Bake(QueueAuthoring authoring)
        {
            AddComponent(new QueueState());
            AddComponent(new Queue()
            {
                QueueDirection = authoring.QueueDirection,
                QueueCapacity = authoring.QueueCapacity,
                FacingCarriageNumber = authoring.FacingCarriageNumber
            });
        }
    }
}

public struct Queue : IComponentData
{
    public float3 QueueDirection;
    public int FacingCarriageNumber;
    public int QueueCapacity;
}

public struct QueueState : IComponentData
{
    public Entity FacingCarriage;
    public int QueueSize;
    public bool IsOpen;
}
