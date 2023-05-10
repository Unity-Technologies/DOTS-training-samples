using System;
using UnityEngine;
using Unity.Entities;
using System.Collections.Generic;
using UnityEngine.Serialization;

namespace Metro
{
    public class ConfigAuthoring : MonoBehaviour
    {
        public int NumPassengersPerStation = 100;
        public GameObject PassengerPrefab;
        public GameObject TrainPrefab;
        public float MaxTrainSpeed = 5f;
        public float TrainAcceleration = 0.2f;
        public float TrainUnloadingTime = 2f;
        public float TrainLoadingTime = 2f;

        class Baker : Baker<ConfigAuthoring>
        {
            public override void Bake(ConfigAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.None);
                AddComponent(entity, new Config
                {
                    NumPassengersPerStation = authoring.NumPassengersPerStation,
                    PassengerEntity = GetEntity(authoring.PassengerPrefab, TransformUsageFlags.Dynamic),
                    TrainEntity = GetEntity(authoring.TrainPrefab, TransformUsageFlags.Dynamic),
                    MaxTrainSpeed = authoring.MaxTrainSpeed,
                    TrainAcceleration = authoring.TrainAcceleration,
                    LoadingTime = authoring.TrainLoadingTime,
                    UnloadingTime = authoring.TrainUnloadingTime
                });
            }
        }

    }
    public struct Config : IComponentData
    {
        public Entity PassengerEntity;
        public int NumPassengersPerStation;
        public Entity TrainEntity;
        public float MaxTrainSpeed;
        public float TrainAcceleration;
        public float UnloadingTime;
        public float LoadingTime;
    }
}
