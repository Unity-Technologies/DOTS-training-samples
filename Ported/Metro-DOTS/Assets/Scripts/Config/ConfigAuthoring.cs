using UnityEngine;
using Unity.Entities;

namespace Metro
{
    public class ConfigAuthoring : MonoBehaviour
    {
        public int NumPassengers = 1000;
        public GameObject PassengerPrefab;
        
        class Baker : Baker<ConfigAuthoring>
        {
            public override void Bake(ConfigAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.None);
                AddComponent(entity, new Config
                {
                    NumPassengers = authoring.NumPassengers,
                    PassengerEntity = GetEntity(authoring.PassengerPrefab, TransformUsageFlags.Dynamic)
                });
            }
        }
        
        public struct Config : IComponentData
        {
            public Entity PassengerEntity;
            public int NumPassengers;
        }
    }
}
