using Assets.Scripts.BlobData;
using Unity.Entities;

public struct Roads : IComponentData
{
    public  BlobAssetReference<RoadAsset> Value;
}
