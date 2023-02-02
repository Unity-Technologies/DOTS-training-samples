using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Serialization;

public class PlatformAuthoring : MonoBehaviour
{
    public List<GameObject> Stairs;
    public GameObject InitialParkedTrain;
    public GameObject PlatformFloor;
    public List<GameObject> Queues;
    public GameObject TrainStopPosition;
    [FormerlySerializedAs("trainDoorOpenSide")] public DoorSide TrainDoorOpenSide = DoorSide.Both;

    class Baker : Baker<PlatformAuthoring>
    {
        public override void Bake(PlatformAuthoring authoring)
        {
            AddComponent(new Platform()
            {
                TrainStopPosition = authoring.TrainStopPosition.transform.position,
                ParkedTrain = GetEntity(authoring.InitialParkedTrain),
                PlatformFloor = GetEntity(authoring.PlatformFloor),
                TrainDoorOpenSide = authoring.TrainDoorOpenSide
            });

            var queueBuffer = AddBuffer<PlatformQueue>();
            queueBuffer.EnsureCapacity(queueBuffer.Length);
            foreach (var queueObject in authoring.Queues)
            {
                queueBuffer.Add(new PlatformQueue(){ Queue = GetEntity(queueObject) });
            }
            
            var platformStairs = AddBuffer<PlatformStairs>();
            foreach (var stairs in authoring.Stairs)
            {
                platformStairs.Add(new PlatformStairs()
                {
                    Stairs = GetEntity(stairs)
                });
            }
        }
    }
}

public struct Platform : IComponentData
{
    public Entity Entity;
    public float3 TrainStopPosition;
    public Entity ParkedTrain;
    public Line Line;
    public Entity PlatformFloor;
    public DoorSide TrainDoorOpenSide;
}

public struct PlatformQueue : IBufferElementData
{
    public Entity Queue;
}

[InternalBufferCapacity(2)]
public struct PlatformStairs : IBufferElementData
{
    public Entity Stairs;
}
