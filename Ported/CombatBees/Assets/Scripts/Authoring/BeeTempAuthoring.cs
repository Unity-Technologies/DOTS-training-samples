using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Transforms;
using UnityEngine;

public class BeeTempAuthoring : MonoBehaviour
{
}


class BeeBaker : Baker<BeeTempAuthoring>
{
    public override void Bake(BeeTempAuthoring authoring)
    {
        AddComponent<LocalToWorld>();
        AddComponent<BeeTempComponent>();
    }
}
