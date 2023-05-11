using System.Collections.Generic;
﻿using Metro;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace Metro
{
    public class TrainAuthoring : MonoBehaviour
    {
        public List<GameObject> Seats;

        class Baker : Baker<TrainAuthoring>
        {
            public override void Bake(TrainAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.Dynamic);
                AddComponent(entity, new Train()
                {
                    OnPlatformA = false,
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
                var buffer = AddBuffer<SeatingComponentElement>(entity);

                for (int i = 0; i < authoring.Seats.Count; i++)
                {
                    float3 localOffsetToTrainGo = authoring.gameObject.transform.position - authoring.Seats[i].transform.position;
                    buffer.Add(new SeatingComponentElement() { SeatPosition = new float3(localOffsetToTrainGo.z, math.abs(localOffsetToTrainGo.y), localOffsetToTrainGo.x) });
                }
            }
        }
    }
}
