using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

public class PlatformAuthoring : MonoBehaviour
{
    public float3 TrainStopPosition;

    class Baker : Baker<PlatformAuthoring>
    {
        public override void Bake(PlatformAuthoring authoring)
        {
            AddComponent(new Platform()
            {
                TrainStopPosition = authoring.TrainStopPosition
            });
        }
    }
}

struct Platform : IComponentData
{
    public float3 TrainStopPosition;
    public NativeList<Entity> Queues;
    public NativeList<Entity> Stairs;
    public Entity ParkedTrain;
    public Entity Line;
}
