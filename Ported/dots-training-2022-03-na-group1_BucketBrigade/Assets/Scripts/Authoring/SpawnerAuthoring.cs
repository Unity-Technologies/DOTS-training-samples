using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;
using UnityGameObject = UnityEngine.GameObject;
using UnityRangeAttribute = UnityEngine.RangeAttribute;
using UnityMonoBehaviour = UnityEngine.MonoBehaviour;

public class SpawnerAuthoring : UnityMonoBehaviour
    , IConvertGameObjectToEntity
    , IDeclareReferencedPrefabs
{
    public UnityGameObject FetcherPrefab;
    public UnityGameObject CaptainPrefab;
    public UnityGameObject FullBucketWorkerPrefab;
    public UnityGameObject EmptyBucketWorkerPrefab;
    public UnityGameObject OmniWorkerPrefab;
    public UnityGameObject WaterPoolPrefab;
    public UnityGameObject FlameCellPrefab;
    public UnityGameObject BucketPrefab;

    public int TeamCount = 2;
    public int OmniWorkerCount = 0;
    public int MembersCount = 23;
    public int WaterCount = 20;
    public int BucketCount = 100;
    public int MinWaterSupplyCount = 50;
    public int MaxWaterSupplyCount = 100;

    [Header("Fire Simulation")]
    public bool TurnOffFireRendering;
    public int FireDimension = 50;
    public float FirePropagationSpeed = 0.05f;

    // This function is required by IDeclareReferencedPrefabs
    public void DeclareReferencedPrefabs(List<UnityGameObject> referencedPrefabs)
    {
        // Conversion only converts the GameObjects in the scene.
        // This function allows us to inject extra GameObjects,
        // in this case prefabs that live in the assets folder.
        referencedPrefabs.Add(FetcherPrefab);
        referencedPrefabs.Add(CaptainPrefab);
        referencedPrefabs.Add(FullBucketWorkerPrefab);
        referencedPrefabs.Add(EmptyBucketWorkerPrefab);
        referencedPrefabs.Add(OmniWorkerPrefab);
        referencedPrefabs.Add(WaterPoolPrefab);
        referencedPrefabs.Add(FlameCellPrefab);
        referencedPrefabs.Add(BucketPrefab);
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
            CaptainPrefab = conversionSystem.GetPrimaryEntity(CaptainPrefab),
            FullBucketWorkerPrefab = conversionSystem.GetPrimaryEntity(FullBucketWorkerPrefab),
            EmptyBucketWorkerPrefab = conversionSystem.GetPrimaryEntity(EmptyBucketWorkerPrefab),
            OmniWorkerPrefab = conversionSystem.GetPrimaryEntity(OmniWorkerPrefab),
            WaterPoolPrefab = conversionSystem.GetPrimaryEntity(WaterPoolPrefab),
            FlameCellPrefab = TurnOffFireRendering ? Entity.Null : conversionSystem.GetPrimaryEntity(FlameCellPrefab),
            BucketPrefab = conversionSystem.GetPrimaryEntity(BucketPrefab),
            TeamCount = TeamCount,
            OmniWorkerCount = OmniWorkerCount,
            MembersCount = MembersCount,
            FireDimension = FireDimension,
            WaterCount = WaterCount,
            BucketCount = BucketCount,
            MinWaterSupplyCount = MinWaterSupplyCount,
            MaxWaterSupplyCount = MaxWaterSupplyCount,
            firePropagationSpeed = this.FirePropagationSpeed
        });
    }
}
