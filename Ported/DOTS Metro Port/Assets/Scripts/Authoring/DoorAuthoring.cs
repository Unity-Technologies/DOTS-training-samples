using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Mathematics;

public class DoorAuthoring : MonoBehaviour
{
    public bool IsLeftDoor = false;

}

public class DoorBaker : Baker<DoorAuthoring>
{
    public override void Bake(DoorAuthoring authoring)
    {
        var tranform = authoring.GetComponent<Transform>();

        float offset = authoring.IsLeftDoor ? -0.3f : 0.3f;

        AddComponent(new Door
        {
            //public Entity Wagon;
            DoorState = 0.0f,
            StartPosition = tranform.localPosition,
            EndPosition = tranform.localPosition + Vector3.right * offset,
            Open = false
        });
    }
}
