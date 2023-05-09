using Components;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace Metro
{
    public class TrainAuthoring : MonoBehaviour
    {
        class Baker : Baker<TrainAuthoring>
        {
            public override void Bake(TrainAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.None);
                AddComponent(entity, new TrainIDComponent()
                {
                    TrainID = 0,
                    TrackID = 0,
                    TrackPointIndex = 0,
                    Forward = true,
                    Offset = new float3(0, 0, 0)
                });
                AddComponent(entity, new EnRouteComponent());
                AddComponent(entity, new LoadingComponent());
            }
        }
    }
}
