using Unity.Entities;
using System.Collections.Generic;
using UnityEngine;
using Unity.Collections;
using UnityGameObject = UnityEngine.GameObject;
using UnityRangeAttribute = UnityEngine.RangeAttribute;
using UnityMonoBehaviour = UnityEngine.MonoBehaviour;

public class PassengerSpawnerAuthoring : UnityMonoBehaviour, IConvertGameObjectToEntity, IDeclareReferencedPrefabs
{
    public UnityGameObject PassengerPrefab;
    public int TotalCount = 1000;
    public Color[] colors;

    // This function is required by IDeclareReferencedPrefabs
    public void DeclareReferencedPrefabs(List<UnityGameObject> referencedPrefabs)
    {
        referencedPrefabs.Add(PassengerPrefab);
    }

    // This function is required by IConvertGameObjectToEntity
    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        BlobBuilder builder = new BlobBuilder(Allocator.TempJob);
        ref ColorsBlob data = ref builder.ConstructRoot<ColorsBlob>();
        BlobBuilderArray<Color> colorArray = builder.Allocate(ref data.colors, colors.Length);
        for (int i = 0; i < colorArray.Length; i++) {
            colorArray[i] = colors[i];
        }
        
        BlobAssetReference<ColorsBlob> blobReference = builder.CreateBlobAssetReference<ColorsBlob>(Allocator.Persistent);
        builder.Dispose();
        
        // GetPrimaryEntity fetches the entity that resulted from the conversion of
        // the given GameObject, but of course this GameObject needs to be part of
        // the conversion, that's why DeclareReferencedPrefabs is important here.
        dstManager.AddComponentData(entity, new PassengerSpawner
        {
            PassengerPrefab = conversionSystem.GetPrimaryEntity(PassengerPrefab),
            TotalCount = TotalCount,
            colorsBlob = blobReference
        });
    }
}