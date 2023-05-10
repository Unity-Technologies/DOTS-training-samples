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

        class Baker : Baker<ConfigAuthoring>
        {
            public override void Bake(ConfigAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.None);
                AddComponent(entity, new Config
                {
                    NumPassengersPerStation = authoring.NumPassengersPerStation,
                    PassengerEntity = GetEntity(authoring.PassengerPrefab, TransformUsageFlags.Dynamic)
                });
            }
        }

    }
    public struct Config : IComponentData
    {
        public Entity PassengerEntity;
        public int NumPassengersPerStation;
    }
}
