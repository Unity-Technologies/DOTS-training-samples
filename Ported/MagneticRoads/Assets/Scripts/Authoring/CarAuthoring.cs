using System.Collections;
using System.Collections.Generic;
using Components;
using Unity.Entities;
using UnityEngine;

public class CarAuthoring : MonoBehaviour
{
    public float Speed;
}

public class CarBaker : Baker<CarAuthoring>
{
    public override void Bake(CarAuthoring authoring)
    {
        AddComponent(new Car
        {
            Speed = authoring.Speed,
            Track = Entity.Null,
            Position = authoring.transform.position,
            Rotation = authoring.transform.rotation,
            Direction = authoring.transform.forward
        });
    }
}
