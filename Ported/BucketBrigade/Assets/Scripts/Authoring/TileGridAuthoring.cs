using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

class TileGridAuthoring : UnityEngine.MonoBehaviour
{
}

class TileGridBaker : Baker<TileGridAuthoring>
{
    public override void Bake(TileGridAuthoring authoring)
    {
        AddComponent(new TileGrid() { entity = GetEntity(authoring)});
    }
}
