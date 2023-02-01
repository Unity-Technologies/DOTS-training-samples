using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

public class PlatformAuthoring : MonoBehaviour
{
    public GameObject Stairs;
    public GameObject InitialParkedTrain;
    public List<GameObject> Queues;
    public float3 TrainStopPosition;

    class Baker : Baker<PlatformAuthoring>
    {
        public override void Bake(PlatformAuthoring authoring)
        {
            AddComponent(new Platform()
            {
                TrainStopPosition = authoring.TrainStopPosition,
                Stairs = GetEntity(authoring.Stairs),
                ParkedTrain = GetEntity(authoring.InitialParkedTrain)
            });

            var queueBuffer = AddBuffer<PlatformQueue>();
            queueBuffer.EnsureCapacity(queueBuffer.Length);
            foreach (var queueObject in authoring.Queues)
            {
                queueBuffer.Add(new PlatformQueue(){ Queue = GetEntity(queueObject) });
            }
        }
    }
}

public struct Platform : IComponentData
{
    public float3 TrainStopPosition;
    public Entity Stairs;
    public Entity ParkedTrain;
    public Line Line;
}

public struct PlatformQueue : IBufferElementData
{
    public Entity Queue;
}
