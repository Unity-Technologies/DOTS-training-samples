using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

public class PlatformAuthoring : MonoBehaviour
{
    public GameObject Stairs;
    public float3 TrainStopPosition;

    class Baker : Baker<PlatformAuthoring>
    {
        public override void Bake(PlatformAuthoring authoring)
        {
            AddComponent(new Platform()
            {
                TrainStopPosition = authoring.TrainStopPosition,
                Stairs = GetEntity(authoring.Stairs)
            });
        }
    }
}

struct Platform : IComponentData
{
    public float3 TrainStopPosition;
    public NativeList<Entity> Queues;
    public Entity Stairs;
    public Entity ParkedTrain;
    public Entity Line;
}
