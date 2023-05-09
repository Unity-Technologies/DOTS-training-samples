using UnityEngine;
using Unity.Entities;

namespace Metro
{
    public class StationsConfigAuthoring : MonoBehaviour
    {
        public GameObject StationPrefab;
        public int NumStations = 10;
        public float Spacing;

        class Baker : Baker<StationsConfigAuthoring>
        {
            public override void Bake(StationsConfigAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.None);
                AddComponent(entity, new StationConfig
                {
                    StationEntity = GetEntity(authoring.StationPrefab, TransformUsageFlags.Dynamic),
                    Spacing = authoring.Spacing,
                    NumStations = authoring.NumStations
                });
            }
        }
    }

    public struct StationConfig : IComponentData
    {
        public Entity StationEntity;
        public Entity TrainEntity;
        public float Spacing;
        public int NumStations;
    }
}
