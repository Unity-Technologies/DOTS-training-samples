using System;
using UnityEngine;
using Unity.Entities;
using System.Collections.Generic;
using Unity.Mathematics;

namespace Metro
{
    public class ConfigAuthoring : MonoBehaviour
    {
        public int NumPassengers = 1000;
        public GameObject PassengerPrefab;
        public GameObject TrainPrefab;
        public float MaxTrainSpeed = 5f;
        public float3[] TrackPoints;

        class Baker : Baker<ConfigAuthoring>
        {
            public override void Bake(ConfigAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.None);
                AddComponent(entity, new Config
                {
                    NumPassengers = authoring.NumPassengers,
                    PassengerEntity = GetEntity(authoring.PassengerPrefab, TransformUsageFlags.Dynamic),
                    TrainEntity = GetEntity(authoring.TrainPrefab, TransformUsageFlags.Dynamic),
                    MaxTrainSpeed = authoring.MaxTrainSpeed
                });
                var trackPoints = AddBuffer<TrackPoint>(entity);
                trackPoints.Length = authoring.TrackPoints.Length;
                for (int i = 0; i < authoring.TrackPoints.Length; i++)
                {
                    trackPoints[i] = new TrackPoint(){ Value = authoring.TrackPoints[i]};
                }
            }
        }

    }
    public struct Config : IComponentData
    {
        public Entity PassengerEntity;
        public Entity TrainEntity;
        public int NumPassengers;
        public float MaxTrainSpeed;
    }
    
    [InternalBufferCapacity(5)]
    public struct TrackPoint : IBufferElementData
    {
        public float3 Value;
    }
}
