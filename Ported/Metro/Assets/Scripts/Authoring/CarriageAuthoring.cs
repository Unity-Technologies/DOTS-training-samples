using Unity.Collections;
using Unity.Entities;
using UnityEngine;

public class CarriageAuthoring : MonoBehaviour
{
    class Baker : Baker<CarriageAuthoring>
    {
        public override void Bake(CarriageAuthoring authoring)
        {
            AddComponent<Carriage>();
        }
    }
}

struct Carriage : IComponentData
{
    public NativeList<Entity> Seats;
    public NativeList<Entity> LeftDoors;
    public NativeList<Entity> RightDoors;
}
