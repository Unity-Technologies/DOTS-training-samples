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
            NativeList<Entity> doors = new NativeList<Entity>(Allocator.Persistent);
            foreach (GameObject door in authoring.Doors)
            {
                doors.Add(GetEntity(door));
            }

            AddComponent(new Carriage()
            {
                CarriageNumber = authoring.CarriageNumber,
                LeftDoors = doors
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

public struct Carriage : IComponentData
{
    public int CarriageNumber;
    public int ownerTrainID;
    public NativeList<Entity> LeftDoors;//NEED TO USE A BUFFER
    public NativeList<Entity> RightDoors;//NEED TO USE A BUFFER
    public Entity CurrentPlatform; 
}

struct CarriageSeat : IBufferElementData
{
    public Entity Seat;
}