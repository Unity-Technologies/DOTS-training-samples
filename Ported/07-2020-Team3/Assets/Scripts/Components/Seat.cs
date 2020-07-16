using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public struct Seat : IComponentData
{
    public Entity car;
    public int indexInCar;
    public Entity occupiedBy;
}
