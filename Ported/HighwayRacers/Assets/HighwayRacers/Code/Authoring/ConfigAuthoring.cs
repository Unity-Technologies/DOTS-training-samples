using Unity.Entities;
using UnityEngine;

public class ConfigAuthoring : MonoBehaviour
{
    public int DesiredCarNumber = 1000;
    public GameObject carPrefab;
    public int MaxCarSpawnPerFrame = 10;

    class Baker : Baker<ConfigAuthoring>
    {
        public override void Bake(ConfigAuthoring authoring)
        {
            // Each authoring field corresponds to a component field of the same name.
            AddComponent(new Config
            {
                DesiredCarNumber = authoring.DesiredCarNumber,
                CarPrefab = GetEntity(authoring.carPrefab),
                MaxCarSpawnPerFrame = authoring.MaxCarSpawnPerFrame
            });
        }
    }
}

public struct Config : IComponentData
{
    public int DesiredCarNumber;
    public Entity CarPrefab;
    public int MaxCarSpawnPerFrame;
}