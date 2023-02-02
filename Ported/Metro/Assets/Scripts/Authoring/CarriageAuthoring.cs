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
                Entity = GetEntity(authoring),
                CarriageNumber = authoring.CarriageNumber,
            });

            var seatBuffer = AddBuffer<CarriageSeat>();
            seatBuffer.EnsureCapacity(authoring.Seats.Count);
            foreach (var seatGameObject in authoring.Seats)
            {
                seatBuffer.Add(new() { Seat = GetEntity(seatGameObject) });
            }
            
            var doorBuffer = AddBuffer<DoorEntity>();
            doorBuffer.EnsureCapacity(authoring.Doors.Count);
            foreach (var doorObj in authoring.Doors)
            {
                doorBuffer.Add(new() { doorEntity = GetEntity(doorObj) });
            }
        }
    }
}

public struct Carriage : IComponentData
{
    public int CarriageNumber;
    public Entity Entity;
    public Entity ownerTrainID;
    public Entity CurrentPlatform; 
}

struct CarriageSeat : IBufferElementData
{
    public Entity Seat;
}

public struct DoorEntity : IBufferElementData
{
    public Entity doorEntity;
}