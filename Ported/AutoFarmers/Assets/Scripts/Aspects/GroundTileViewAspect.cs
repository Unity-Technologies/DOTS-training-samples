using Unity.Entities;
using Unity.Rendering;

public readonly partial struct GroundTileAspect : IAspect<GroundTileAspect>
{
    public readonly Entity self;
    readonly RefRO<GroundTileView> tileViewRef;
    readonly RefRO<MaterialMeshInfo> tileMaterialMeshInfo;

    public GroundTileView tileView => tileViewRef.ValueRO;
    public MaterialMeshInfo meshInfo => tileMaterialMeshInfo.ValueRO;
}
