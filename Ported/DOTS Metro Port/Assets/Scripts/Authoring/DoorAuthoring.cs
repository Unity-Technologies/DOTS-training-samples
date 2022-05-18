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
        var transform = authoring.GetComponent<Transform>();

        float offset = authoring.IsLeftDoor ? -0.3f : 0.3f;

        Entity temp = GetEntity(transform.parent.parent.gameObject);

        AddComponent(new Door
        {
            Carriage = temp,
            StartPosition = transform.localPosition,
            EndPosition = transform.localPosition + Vector3.right * offset
        }); ;
    }
}
