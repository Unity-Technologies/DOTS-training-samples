using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;

public class CarriageAuthoring : MonoBehaviour
{
    public List<GameObject> Doors;
    public List<GameObject> Seats;
    public int CarriageNumber;

    class Baker : Baker<CarriageAuthoring>
    {
        public override void Bake(CarriageAuthoring authoring)
        {
            AddComponent(new Carriage()
            {
                CarriageNumber = authoring.CarriageNumber
            });

            var seatBuffer = AddBuffer<CarriageSeat>();
            seatBuffer.EnsureCapacity(authoring.Seats.Count);
            foreach (var seatGameObject in authoring.Seats)
            {
                seatBuffer.Add(new() { Seat = GetEntity(seatGameObject) });
            }
        }
    }
}

struct Carriage : IComponentData
{
    public int CarriageNumber;
    public NativeList<Entity> LeftDoors;
    public NativeList<Entity> RightDoors;
}

struct CarriageSeat : IBufferElementData
{
    public Entity Seat;
}