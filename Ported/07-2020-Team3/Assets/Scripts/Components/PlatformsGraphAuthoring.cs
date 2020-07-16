using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;

public struct MyBlobData
{
    public float SomeFloat;
}

public struct PlatformsGraph : IComponentData
{
    public BlobAssetReference<MyBlobData> Value;
    public ref MyBlobData Resolve => ref Value.Value;
    public bool IsCreated => Value.IsCreated;
}

public class PlatformsGraphAuthoring : MonoBehaviour, IConvertGameObjectToEntity
{
    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        var blobBuilder = new BlobBuilder(Allocator.Temp);
        ref var myBlobData = ref blobBuilder.ConstructRoot<MyBlobData>();
        myBlobData.SomeFloat = 10f;
        var myBlobAsset = blobBuilder.CreateBlobAssetReference<MyBlobData>(Allocator.Persistent);
        dstManager.AddComponentData(entity, new PlatformsGraph
        {
            Value = myBlobAsset
        });
    }
}
