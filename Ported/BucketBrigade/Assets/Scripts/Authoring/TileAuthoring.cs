using Unity.Entities;
using Unity.Rendering;
using Unity.Transforms;

class TileAuthoring : UnityEngine.MonoBehaviour
{
}

class TileBaker : Baker<TileAuthoring>
{
    public override void Bake(TileAuthoring authoring)
    {
        AddComponent<Tile>();
        AddComponent<URPMaterialPropertyBaseColor>();
        AddComponent<NonUniformScale>();
    }
}
