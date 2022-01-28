using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

public struct PassengerSpawner : IComponentData
{
    public Entity PassengerPrefab;
    public int TotalCount;

    public BlobAssetReference<ColorsBlob> colorsBlob;
}
