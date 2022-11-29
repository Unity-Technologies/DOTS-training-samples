using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public class PassengerAuthoring : MonoBehaviour
{
}

class PassengerBaker : Baker<PassengerAuthoring>
{
    public override void Bake(PassengerAuthoring authoring)
    {
        AddComponent<Passenger>();
    }
}