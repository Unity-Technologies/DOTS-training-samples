using UnityEngine;
using Unity.Entities;

namespace Metro
{
    public class ConfigAuthoring : MonoBehaviour
    {
        public int NumPassengers = 1000;
        
        class Baker : Baker<ConfigAuthoring>
        {
            public override void Bake(ConfigAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.None);
                AddComponent(entity, new Config
                {
                    NumPassengers = authoring.NumPassengers
                });
            }
        }
        
        public struct Config : IComponentData
        {
            public int NumPassengers;
        }
    }
}
