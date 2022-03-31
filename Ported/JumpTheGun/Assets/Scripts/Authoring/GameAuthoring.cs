using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Transforms;
using Unity.Rendering;
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

    [Range(1, 100000)]
    public int TankCount = 1;

    [Range(1, 1000)]
    public int TerrainWidth = 10;

    [Range(1, 1000)]
    public int TerrainLength = 10;

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
        var brickPrefabEntity = conversionSystem.GetPrimaryEntity(BrickPrefab);
        var tankEntityPrefab = conversionSystem.GetPrimaryEntity(TankPrefab);
        var cannonBallEntityPrefab = conversionSystem.GetPrimaryEntity(CannonBallPrefab);

        // GetPrimaryEntity fetches the entity that resulted from the conversion of
        // the given GameObject, but of course this GameObject needs to be part of
        // the conversion, that's why DeclareReferencedPrefabs is important here.
        dstManager.AddComponentData(entity, new EntityPrefabHolder
        {
            BrickEntityPrefab = brickPrefabEntity,
            TankEntityPrefab = tankEntityPrefab,
            CannonBallEntityPrefab = cannonBallEntityPrefab,
        });
        dstManager.AddComponentData(entity, new TerrainData
        {
            MinTerrainHeight = 2.5f,
            MaxTerrainHeight = 5.5f,
            TerrainWidth = TerrainWidth,
            TerrainLength = TerrainLength,
        });
        dstManager.AddComponentData(brickPrefabEntity, new NonUniformScale());
        dstManager.AddComponentData(brickPrefabEntity, new BrickTag());
        dstManager.AddComponent<URPMaterialPropertyBaseColor>(brickPrefabEntity);

        dstManager.AddComponentData(entity, new TankData
        {
            TankCount = TankCount,
        });
        dstManager.AddComponentData(tankEntityPrefab, new Tank());

        dstManager.AddComponentData(cannonBallEntityPrefab, new ParabolaData {duration = 0.001f});
        dstManager.AddComponent<CannonBallTag>(cannonBallEntityPrefab);
        dstManager.AddComponent<NormalizedTime>(cannonBallEntityPrefab);
    }
}