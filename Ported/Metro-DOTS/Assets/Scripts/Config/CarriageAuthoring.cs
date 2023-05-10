using UnityEngine;
using Unity.Entities;

namespace Metro
{
    public class CarriageAuthoring : MonoBehaviour
    {
        [Range(1, 27)]
        public int CarriageCapacity;

        public GameObject PortDoors;
        public GameObject StarboardDoors;

        private class Baker : Baker<CarriageAuthoring>
        {
            public override void Bake(CarriageAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.Dynamic);
                AddComponent(entity, new Carriage
                {
                    CarriageCapacity = authoring.CarriageCapacity,

                    PortDoors = GetEntity(authoring.PortDoors, TransformUsageFlags.Dynamic),
                    StarboardDoors = GetEntity(authoring.StarboardDoors, TransformUsageFlags.Dynamic)
                });
            }
        }
    }
    
    public struct Carriage : IComponentData
    {
        public Entity PortDoors;
        public Entity StarboardDoors;

        public int CarriageCapacity;
    }
}