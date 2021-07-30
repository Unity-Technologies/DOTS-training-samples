using System.Collections.Generic;
using Unity.Entities;
using UnityGameObject = UnityEngine.GameObject;
using UnityRangeAttribute = UnityEngine.RangeAttribute;
using UnityMonoBehaviour = UnityEngine.MonoBehaviour;

public class ObstacleBucketIndicesAuthoring : UnityMonoBehaviour
    , IConvertGameObjectToEntity
    , IDeclareReferencedPrefabs
{
    [UnityRange(1, 128)] public int bucketResolution = 64;

    // This function is required by IDeclareReferencedPrefabs
    public void DeclareReferencedPrefabs(List<UnityGameObject> referencedPrefabs)
    {
        // Conversion only converts the GameObjects in the scene.
        // This function allows us to inject extra GameObjects,
        // in this case prefabs that live in the assets folder.
    }

    public void Convert(Entity entity, EntityManager dstManager
        , GameObjectConversionSystem conversionSystem)
    {
        //dstManager.AddComponentData(entity, new ObstacleBucketEntity()
        //{
        //    BucketResolution = bucketResolution
        //});
        //var bucketIndicesBuffer = dstManager.AddBuffer<ObstacleBucketIndices>(entity);
    }
}

