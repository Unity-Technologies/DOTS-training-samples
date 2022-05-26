using Unity.Entities;
using Unity.Mathematics;

class GroundTileAuthoring : UnityEngine.MonoBehaviour
{
}

class GroundTileViewBaker : Baker<GroundTileAuthoring>
{
    public override void Bake(GroundTileAuthoring authoring)
    {
        AddComponent(new GroundTileView {});
    }
}