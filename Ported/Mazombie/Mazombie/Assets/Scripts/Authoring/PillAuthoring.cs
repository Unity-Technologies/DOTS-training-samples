using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public class PillAuthoring : MonoBehaviour
{
}

public class PillBaker : Baker<PillAuthoring>
{
    public override void Bake(PillAuthoring authoring)
    {
        AddComponent(new Pill
        {
        });
    }
}