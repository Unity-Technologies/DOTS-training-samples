using System.Collections.Generic;
using Unity.Entities;
using Unity.Transforms;
using UnityGameObject = UnityEngine.GameObject;
using UnityRangeAttribute = UnityEngine.RangeAttribute;
using UnityMonoBehaviour = UnityEngine.MonoBehaviour;

public class GameAuthoring : UnityMonoBehaviour
    , IConvertGameObjectToEntity
    , IDeclareReferencedPrefabs
{
    public UnityGameObject BrickPrefab;
    public UnityGameObject TankPrefab;
    public UnityGameObject CannonBallPrefab;

    // This function is required by IDeclareReferencedPrefabs
    public void DeclareReferencedPrefabs(List<UnityGameObject> referencedPrefabs)
    {
        // Conversion only converts the GameObjects in the scene.
        // This function allows us to inject extra GameObjects,
        // in this case prefabs that live in the assets folder.
        referencedPrefabs.Add(BrickPrefab);
        referencedPrefabs.Add(TankPrefab);
        referencedPrefabs.Add(CannonBallPrefab);
    }
    
    // This function is required by IConvertGameObjectToEntity
    public void Convert(Entity entity, EntityManager dstManager
        , GameObjectConversionSystem conversionSystem)
    {
        // GetPrimaryEntity fetches the entity that resulted from the conversion of
        // the given GameObject, but of course this GameObject needs to be part of
        // the conversion, that's why DeclareReferencedPrefabs is important here.
        dstManager.AddComponentData(entity, new EntityPrefabHolder
        {
            BrickEntityPrefab = conversionSystem.GetPrimaryEntity(BrickPrefab),
            TankEntityPrefab = conversionSystem.GetPrimaryEntity(TankPrefab),
            CannonBallEntityPrefab = conversionSystem.GetPrimaryEntity(CannonBallPrefab),
        });

        dstManager.AddComponentData(conversionSystem.GetPrimaryEntity(BrickPrefab), new NonUniformScale());
        
        dstManager.AddComponentData(entity, new TerrainData
        {
            MinTerrainHeight = 2.5f,
            MaxTerrainHeight = 5.5f,
            TerrainWidth = 500,
            TerrainLength = 500,
        });
    }
}