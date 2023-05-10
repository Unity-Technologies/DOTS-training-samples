using System;
using UnityEngine;
using Unity.Entities;

namespace Metro
{
    public class ConfigAuthoring : MonoBehaviour
    {
        public int NumPassengers = 1000;
        public GameObject PassengerPrefab;
        public GameObject TrainPrefab;
        public float MaxTrainSpeed = 5f;
        public float TrainAcceleration = 0.2f;

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
                    MaxTrainSpeed = authoring.MaxTrainSpeed,
                    TrainAcceleration = authoring.TrainAcceleration
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
        public float TrainAcceleration;

        public static float TrainWaitingTime; // The total duration that a train waits at a station
        public static float DoorAnimationTIme; // The duration that it takes for a door to open or close
    }
}
