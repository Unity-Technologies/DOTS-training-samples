using Unity.Entities;
using UnityMonoBehavior = UnityEngine.MonoBehaviour;

public struct Settings : IComponentData
{
    public BlobAssetReference<SettingsBlobAsset> SettingsBlobRef;
}
