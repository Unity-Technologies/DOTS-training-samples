using System;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using Unity.Transforms;
using UnityEngine;

public class CarridgeInfo : MonoBehaviour
{
    public List<DoorInfoGameObjects> leftDoors;
    public List<DoorInfoGameObjects> rightDoors;
}


public struct Carriage : IComponentData
{
    public Train ownerTrain;
    public NativeList<Entity> Seats;
    public NativeList<Entity> LeftDoors;
    public NativeList<Entity> RightDoors;
}

[Serializable]
public struct DoorInfoGameObjects
{
    public GameObject door1;
    public GameObject door2;
}

