using System.Collections;
using System.Collections.Generic;
using Components;
using Unity.Entities;
using UnityEngine;




public class CarAuthoring : MonoBehaviour
{
    public float Speed;
    public float SafeDistance;
    public UnityEngine.GameObject TestTrack;
}

public class CarBaker : Baker<CarAuthoring>
{
    public override void Bake(CarAuthoring authoring)
    {
        AddComponent(new Car
        {
            Speed = authoring.Speed,
            Track = GetEntity(authoring.TestTrack),
            SafeDistance = authoring.SafeDistance
        });
    }
}
