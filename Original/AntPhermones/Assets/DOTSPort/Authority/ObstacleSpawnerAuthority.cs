using UnityEngine;
using Unity.Entities;
using UnityEngine.Serialization;

public class ObstacleSpawnerAuthority : MonoBehaviour
{
    public int ObstacleRingCount;
    public float ObstaclePercentPerRing;
    public float ObstacleRadius;  // TODO deprecate this, maybe?

    class Baker : Baker<ObstacleSpawnerAuthority>
    {
        public override void Bake(ObstacleSpawnerAuthority authoring)
        {
            var entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity, new ObstacleSpawner()
            {
                ObstacleRingCount = authoring.ObstacleRingCount,
                ObstaclePercentPerRing = authoring.ObstaclePercentPerRing,
                ObstacleRadius = authoring.ObstacleRadius
            });
        }
    }
}
