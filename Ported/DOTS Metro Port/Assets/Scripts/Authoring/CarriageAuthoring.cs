using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Rendering;

public class CarriageAuthoring : MonoBehaviour
{

}

public class CarriageBaker : Baker<CarriageAuthoring>
{
    public override void Bake(CarriageAuthoring authoring)
    {
        AddComponent<Carriage>();
        var buffer = AddBuffer<ChildrenWithRenderer>().Reinterpret<Entity>();
        foreach (var renderer in GetComponentsInChildren<UnityEngine.MeshRenderer>())
        {
            buffer.Add(GetEntity(renderer));
        }
    }
}
