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
            AddComponent(new QueueState()
            {
                FacingCarriageNumber = authoring.FacingCarriageNumber
            });
            
            AddComponent(new Queue()
            {
                QueueDirection = authoring.QueueDirection,
                QueueDirectionOrthogonal = new float3(authoring.QueueDirection.z, authoring.QueueDirection.y, authoring.QueueDirection.x),
                QueueCapacity = authoring.QueueCapacity
            });
        }
    }
}

public struct Queue : IComponentData
{
    public float3 QueueDirection;
    public float3 QueueDirectionOrthogonal;
    public int QueueCapacity;
}

public struct QueueState : IComponentData
{
    public Entity FacingCarriage;
    public int QueueSize;
    public bool IsOpen;
    public int FacingCarriageNumber;
}
