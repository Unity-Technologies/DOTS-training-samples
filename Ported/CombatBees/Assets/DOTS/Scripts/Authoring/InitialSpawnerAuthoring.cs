using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;
using Unity.Transforms;
using UnityCamera = UnityEngine.Camera;
using UnityGameObject = UnityEngine.GameObject;
using UnityInput = UnityEngine.Input;
using UnityKeyCode = UnityEngine.KeyCode;
using UnityMeshRenderer = UnityEngine.MeshRenderer;
using UnityMonoBehaviour = UnityEngine.MonoBehaviour;
using UnityRangeAttribute = UnityEngine.RangeAttribute;

public class InitialSpawnerAuthoring : UnityMonoBehaviour
    , IConvertGameObjectToEntity
    , IDeclareReferencedPrefabs
{
    public UnityGameObject BeePrefab;
    [UnityRange(0, 1000)] public int BeeCount;

    public UnityGameObject ResourcePrefab;
    [UnityRange(0, 1000)] public int ResourceCount;

    [UnityRange(0, 100)] public int MaxSpeed = 1;

    // This function is required by IDeclareReferencedPrefabs
    public void DeclareReferencedPrefabs(List<UnityGameObject> referencedPrefabs)
    {
        // Conversion only converts the GameObjects in the scene.
        // This function allows us to inject extra GameObjects,
        // in this case prefabs that live in the assets folder.
        referencedPrefabs.Add(BeePrefab);
        referencedPrefabs.Add(ResourcePrefab);
    }

    public void Convert(Entity entity, EntityManager dstManager
        , GameObjectConversionSystem conversionSystem)
    {
        // GetPrimaryEntity fetches the entity that resulted from the conversion of
        // the given GameObject, but of course this GameObject needs to be part of
        // the conversion, that's why DeclareReferencedPrefabs is important here.
        dstManager.AddComponentData(entity, new InitialSpawner
        {
            BeePrefab = conversionSystem.GetPrimaryEntity(BeePrefab),
            BeeCount = BeeCount,
            ResourcePrefab = conversionSystem.GetPrimaryEntity(ResourcePrefab),
            ResourceCount = ResourceCount,
            MaxSpeed = MaxSpeed
        });
    }
}
