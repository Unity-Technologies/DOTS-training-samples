using UnityEngine;
using Unity.Entities;

namespace Metro
{
    public class ConfigAuthoring : MonoBehaviour
    {
        public int NumPassengersPerStation = 100;
        public float PassengerSpeed = 2f; // unit per second
        public int MaxPassengerPerQueue = 16;
        public float DistanceBetweenPassengers = 0.4f;
        public GameObject PassengerPrefab;
        public GameObject TrainPrefab;
        public float MaxTrainSpeed = 5f;
        public float MinTrainSpeed = 1f;
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
                    PassengerSpeed = authoring.PassengerSpeed,
                    MaxPassengerPerQueue = authoring.MaxPassengerPerQueue,
                    DistanceBetweenPassengers = authoring.DistanceBetweenPassengers,
                    PassengerEntity = GetEntity(authoring.PassengerPrefab, TransformUsageFlags.Dynamic),
                    TrainEntity = GetEntity(authoring.TrainPrefab, TransformUsageFlags.Dynamic),
                    MaxTrainSpeed = authoring.MaxTrainSpeed,
                    MinTrainSpeed = authoring.MinTrainSpeed,
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
        public int MaxPassengerPerQueue;
        public float DistanceBetweenPassengers;
        public float PassengerSpeed; // unit per second
        public Entity TrainEntity;
        public float MaxTrainSpeed;
        public float MinTrainSpeed;
        public float TrainAcceleration;
        public float UnloadingTime;
        public float LoadingTime;
    }
}
