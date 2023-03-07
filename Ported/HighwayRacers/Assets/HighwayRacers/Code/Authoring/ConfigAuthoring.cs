using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

public class ConfigAuthoring : MonoBehaviour
{
    public int numCars;
    public GameObject carPrefab;
    public float2 SpeedRange;

    class Baker : Baker<ConfigAuthoring>
    {
        public override void Bake(ConfigAuthoring authoring)
        {
            // Each authoring field corresponds to a component field of the same name.
            AddComponent(new Config
            {
                NumCars = authoring.numCars,
                CarPrefab = GetEntity(authoring.carPrefab),
                SpeedRange = authoring.SpeedRange
            });
        }
    }
}

public struct Config : IComponentData
{
    public int NumCars;
    public Entity CarPrefab;
    public float2 SpeedRange;
}