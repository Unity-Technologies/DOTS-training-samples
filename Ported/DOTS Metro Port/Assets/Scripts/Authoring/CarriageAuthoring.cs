using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;

public class CarriageAuthoring : MonoBehaviour
{

}

public class CarriageBaker : Baker<CarriageAuthoring>
{
    public override void Bake(CarriageAuthoring authoring)
    {
        AddComponent<Carriage>();
    }
}
