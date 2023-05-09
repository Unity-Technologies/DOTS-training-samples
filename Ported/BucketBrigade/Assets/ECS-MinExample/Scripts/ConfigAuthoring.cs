using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace Miscellaneous.StateChangeEnableable
{
    public class ConfigAuthoring : MonoBehaviour
    {
        public GameObject Prefab;
        public Transform origin;
        public uint Size;
        public float Radius;

        public class ConfigBaker : Baker<ConfigAuthoring>
        {
            public override void Bake(ConfigAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.None);
				AddComponent(entity, new Config
                {
                    Prefab = GetEntity(authoring.Prefab, TransformUsageFlags.Dynamic),
                    Size = authoring.Size,
                    Radius = authoring.Radius,
                    Origin = new float3(authoring.origin.position)
                });
                AddComponent<Hit>(entity);
            }
        }
    }

    public struct Config : IComponentData
    {
        public Entity Prefab;
        public uint Size;
        public float Radius;
        public float3 Origin;
    }

    public struct Hit : IComponentData
    {
        public float3 Value;
        public bool ChangedThisFrame;
    }
}
