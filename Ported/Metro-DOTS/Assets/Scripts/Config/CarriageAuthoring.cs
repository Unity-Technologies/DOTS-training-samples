using UnityEngine;
using Unity.Entities;

namespace Metro
{
    public class CarriageAuthoring : MonoBehaviour
    {
        [Range(1, 27)]
        public int CarriageCapacity;

        private class Baker : Baker<CarriageAuthoring>
        {
            public override void Bake(CarriageAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.Dynamic);
                AddComponent(entity, new Carriage
                {
                    CarriageCapacity = authoring.CarriageCapacity,
                });
            }
        }
    }
    
    public struct Carriage : IComponentData
    {
        public int CarriageCapacity;
    }
}