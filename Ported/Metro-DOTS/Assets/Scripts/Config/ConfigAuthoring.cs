using UnityEngine;
using Unity.Entities;

namespace Metro
{
    public class ConfigAuthoring : MonoBehaviour
    {
        public int NumPassengers = 1000;
        public GameObject PassengerPrefab;
        public float MinPassengerHeight = 0.6f;
        public float MaxPassengerHeight = 1.2f;

        class Baker : Baker<ConfigAuthoring>
        {
            public override void Bake(ConfigAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.None);
                AddComponent(entity, new Config
                {
                    NumPassengers = authoring.NumPassengers,
                    PassengerEntity = GetEntity(authoring.PassengerPrefab, TransformUsageFlags.Dynamic),
                    MinPassengerHeight = authoring.MinPassengerHeight,
                    MaxPassengerHeight = authoring.MaxPassengerHeight
                });
            }
        }

    }
    public struct Config : IComponentData
    {
        public Entity PassengerEntity;
        public int NumPassengers;
        public float MinPassengerHeight;
        public float MaxPassengerHeight;
    }
}
