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
}
