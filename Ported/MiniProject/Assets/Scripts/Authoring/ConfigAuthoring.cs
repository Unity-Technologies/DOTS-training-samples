using Unity.Entities;
using UnityEngine;

public class ConfigAuthoring : MonoBehaviour
{
    public GameObject ObstaclePrefab;
    public GameObject BallPrefab;
    public int ObstacleCount;
    public int BallCount;
    public float ImpactVelocity;

    class ConfigBaker : Baker<ConfigAuthoring>
    {
        public override void Bake(ConfigAuthoring authoring)
        {
            AddComponent(new Config
            {
                ObstaclePrefab = GetEntity(authoring.ObstaclePrefab),
                BallPrefab = GetEntity(authoring.BallPrefab),
                ObstacleCount = authoring.ObstacleCount,
                BallCount = authoring.BallCount,
                ImpactVelocity = authoring.ImpactVelocity
            });
        }
    }
}

struct Config : IComponentData
{
    public Entity ObstaclePrefab;
    public Entity BallPrefab;
    public int ObstacleCount;
    public int BallCount;
    public float ImpactVelocity;
}