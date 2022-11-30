using Unity.Entities;
using UnityEngine;

public class WallAuthoring : MonoBehaviour
{
}

class WallBaker : Baker<WallAuthoring>
{
    public override void Bake(WallAuthoring authoring)
    {
        AddComponent(new WallComponent());
    }
}