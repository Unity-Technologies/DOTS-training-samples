using System.Collections.Generic;
using Unity.Entities;
using Unity.Entities.Hybrid.Internal;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

[DisallowMultipleComponent]
internal class GameParamsAuthoring : MonoBehaviour, IConvertGameObjectToEntity, IDeclareReferencedPrefabs {
    public GameObject TilePrefab;

    public GameObject CannonBallPrefab;

    public float TerrainMin;

    public float TerrainMax;

    public int2 TerrainDimensions;

    public GameObject CannonPrefab;
    public GameObject CannonBarrelPrefab;

    public int CannonCount;

    public float CannonCooldown;

    public UnityEngine.Color colorA;

    public UnityEngine.Color colorB;

    public GameObject PlayerPrefab;

    public float collisionStepMultiplier;

    public float playerParabolaPrecision;

    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem) {
        GameParams componentData = default(GameParams);
        componentData.TilePrefab = conversionSystem.GetPrimaryEntity(TilePrefab);
        componentData.CannonBallPrefab = conversionSystem.GetPrimaryEntity(CannonBallPrefab);
        componentData.CannonBarrel = conversionSystem.GetPrimaryEntity(CannonBarrelPrefab);
        componentData.TerrainMin = TerrainMin;
        componentData.TerrainMax = TerrainMax;
        componentData.TerrainDimensions = TerrainDimensions;
        componentData.CannonPrefab = conversionSystem.GetPrimaryEntity(CannonPrefab);
        componentData.CannonCount = CannonCount;
        componentData.CannonCooldown = CannonCooldown;
        componentData.colorA = math.float4(colorA.r, colorA.g, colorA.b, colorA.a);
        componentData.colorB = math.float4(colorB.r, colorB.g, colorB.b, colorB.a);
        componentData.PlayerPrefab = conversionSystem.GetPrimaryEntity(PlayerPrefab);
        componentData.collisionStepMultiplier = collisionStepMultiplier;
        componentData.playerParabolaPrecision = playerParabolaPrecision;
        dstManager.AddComponentData(entity, componentData);

        dstManager.RemoveComponent<Translation>(entity);
        dstManager.RemoveComponent<Unity.Transforms.Rotation>(entity);
        dstManager.RemoveComponent<Scale>(entity);
        dstManager.RemoveComponent<NonUniformScale>(entity);
    }

    public void DeclareReferencedPrefabs(List<GameObject> referencedPrefabs) {
        GeneratedAuthoringComponentImplementation.AddReferencedPrefab(referencedPrefabs, TilePrefab);
        GeneratedAuthoringComponentImplementation.AddReferencedPrefab(referencedPrefabs, CannonBallPrefab);
        GeneratedAuthoringComponentImplementation.AddReferencedPrefab(referencedPrefabs, CannonPrefab);
        GeneratedAuthoringComponentImplementation.AddReferencedPrefab(referencedPrefabs, PlayerPrefab);
        GeneratedAuthoringComponentImplementation.AddReferencedPrefab(referencedPrefabs, CannonBarrelPrefab);
    }
}
