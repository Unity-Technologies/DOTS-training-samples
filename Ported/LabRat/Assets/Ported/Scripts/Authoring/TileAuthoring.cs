using Unity.Entities;
using UnityEngine;

public class TileAuthoring : MonoBehaviour
{

}

class TileBaker : Baker<TileAuthoring>
{
    public override void Bake(TileAuthoring authoring)
    {
        AddComponent<TileComponent>();
    }
}