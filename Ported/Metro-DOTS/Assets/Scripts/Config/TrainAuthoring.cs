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
                var entity = GetEntity(TransformUsageFlags.Dynamic);
                AddComponent(entity, new Train()
                {
                    TrainID = 0,
                    TrackID = 0,
                    TrackPointIndex = 1,
                    Forward = true,
                    Offset = new float3(0, 0, 0),
                    Duration = 0
                });

                AddComponent<EnRouteComponent>(entity);
                AddComponent<LoadingComponent>(entity);
                AddComponent<UnloadingComponent>(entity);
                AddComponent<ArrivingComponent>(entity);
                AddComponent<DepartingComponent>(entity);
            }
        }
    }
}
