using System.Collections.Generic;
using Unity.Entities;
using UnityGameObject = UnityEngine.GameObject;
using UnityRangeAttribute = UnityEngine.RangeAttribute;
using UnityMonoBehaviour = UnityEngine.MonoBehaviour;

public class SpawnerAuthoring : UnityMonoBehaviour
    , IConvertGameObjectToEntity
    , IDeclareReferencedPrefabs
{
    public UnityGameObject FetcherPrefab;
    public UnityGameObject FireCaptainPrefab;
    public UnityGameObject WaterCaptainPrefab;
    public UnityGameObject FullBucketWorkerPrefab;
    public UnityGameObject EmptyBucketWorkerPrefab;
    public UnityGameObject OmniWorkerPrefab;
    public UnityGameObject WaterPoolPrefab;
    public UnityGameObject GroundPrefab;
    public UnityGameObject FlameCellPrefab;
    public UnityGameObject BucketPrefab;

    [UnityRange(1, 10)] public int TeamCount = 2;
    [UnityRange(0, 100)] public int OmniWorkerCount = 0;
    [UnityRange(3, 100)] public int MembersCount = 23;
    [UnityRange(1, 200)] public int FireDimension = 100;
    [UnityRange(4, 50)] public int WaterCount = 20;
    [UnityRange(1, 200)] public int BucketCount = 100;
    [UnityRange(1, 200)] public int MinWaterSupplyCount = 50;
    [UnityRange(1, 200)] public int MaxWaterSupplyCount = 100;


    // This function is required by IDeclareReferencedPrefabs
    public void DeclareReferencedPrefabs(List<UnityGameObject> referencedPrefabs)
    {
        // Conversion only converts the GameObjects in the scene.
        // This function allows us to inject extra GameObjects,
        // in this case prefabs that live in the assets folder.
        referencedPrefabs.Add(FetcherPrefab);
        referencedPrefabs.Add(FireCaptainPrefab);
        referencedPrefabs.Add(WaterCaptainPrefab);
        referencedPrefabs.Add(FullBucketWorkerPrefab);
        referencedPrefabs.Add(EmptyBucketWorkerPrefab);
        referencedPrefabs.Add(OmniWorkerPrefab);
        referencedPrefabs.Add(WaterPoolPrefab);
        referencedPrefabs.Add(FlameCellPrefab);
        referencedPrefabs.Add(BucketPrefab);
        referencedPrefabs.Add(GroundPrefab);
    }

    // This function is required by IConvertGameObjectToEntity
    public void Convert(Entity entity, EntityManager dstManager
        , GameObjectConversionSystem conversionSystem)
    {
        // GetPrimaryEntity fetches the entity that resulted from the conversion of
        // the given GameObject, but of course this GameObject needs to be part of
        // the conversion, that's why DeclareReferencedPrefabs is important here.
        dstManager.AddComponentData(entity, new Spawner
        {
            FetcherPrefab = conversionSystem.GetPrimaryEntity(FetcherPrefab),
            FireCaptainPrefab = conversionSystem.GetPrimaryEntity(FireCaptainPrefab),
            WaterCaptainPrefab = conversionSystem.GetPrimaryEntity(WaterCaptainPrefab),
            FullBucketWorkerPrefab = conversionSystem.GetPrimaryEntity(FullBucketWorkerPrefab),
            EmptyBucketWorkerPrefab = conversionSystem.GetPrimaryEntity(EmptyBucketWorkerPrefab),
            OmniWorkerPrefab = conversionSystem.GetPrimaryEntity(OmniWorkerPrefab),
            WaterPoolPrefab = conversionSystem.GetPrimaryEntity(WaterPoolPrefab),
            GroundPrefab = conversionSystem.GetPrimaryEntity(GroundPrefab),
            FlameCellPrefab = conversionSystem.GetPrimaryEntity(FlameCellPrefab),
            BucketPrefab = conversionSystem.GetPrimaryEntity(BucketPrefab),
            TeamCount = TeamCount,
            OmniWorkerCount = OmniWorkerCount,
            MembersCount = MembersCount,
            FireDimension = FireDimension,
            WaterCount = WaterCount,
            BucketCount = BucketCount,
            MinWaterSupplyCount = MinWaterSupplyCount,
            MaxWaterSupplyCount = MaxWaterSupplyCount,
        });
    }
}
