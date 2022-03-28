using System.Collections.Generic;
using Unity.Entities;
using UnityGameObject = UnityEngine.GameObject;
using UnityRangeAttribute = UnityEngine.RangeAttribute;
using UnityMonoBehaviour = UnityEngine.MonoBehaviour;

public class SpawnerAuthoring : UnityMonoBehaviour
    , IConvertGameObjectToEntity
    , IDeclareReferencedPrefabs
{
    public UnityGameObject BotPrefab;
    public UnityGameObject WaterPoolPrefab;
    public UnityGameObject FlameCellPrefab;
    public UnityGameObject BucketPrefab;

    [UnityRange(0, 10)] public int TeamCount;
    [UnityRange(0, 100)] public int MembersCount;
    [UnityRange(0, 200)] public int FireDimension;

    // This function is required by IDeclareReferencedPrefabs
    public void DeclareReferencedPrefabs(List<UnityGameObject> referencedPrefabs)
    {
        // Conversion only converts the GameObjects in the scene.
        // This function allows us to inject extra GameObjects,
        // in this case prefabs that live in the assets folder.
        referencedPrefabs.Add(BotPrefab);
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
            BotPrefab = conversionSystem.GetPrimaryEntity(BotPrefab),
            WaterPoolPrefab = conversionSystem.GetPrimaryEntity(WaterPoolPrefab),
            FlameCellPrefab = conversionSystem.GetPrimaryEntity(FlameCellPrefab),
            BucketPrefab = conversionSystem.GetPrimaryEntity(BucketPrefab),
            TeamCount = TeamCount,
            MembersCount = MembersCount,
            FireDimension = FireDimension,
        });
    }
}
