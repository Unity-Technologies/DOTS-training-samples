using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;

public class WallAuthoring : MonoBehaviour
{

}

class WallBaker : Baker<WallAuthoring>
{
    public override void Bake(WallAuthoring authoring)
    {
        AddComponent(new Obstacle());
    }
}
