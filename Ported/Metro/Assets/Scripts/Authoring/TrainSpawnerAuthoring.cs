using Unity.Entities;
using Unity.Mathematics;
using Unity.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Rendering;
using UnityGameObject = UnityEngine.GameObject;
using UnityMonoBehaviour = UnityEngine.MonoBehaviour;
using UnityMeshRenderer = UnityEngine.MeshRenderer;
public class TrainSpawnerAuthoring : MonoBehaviour, IConvertGameObjectToEntity, IDeclareReferencedPrefabs
{
    public UnityGameObject LeaderPrefab;
    public int NoOfTrains;
    public UnityGameObject FollowerPrefab;
    public int NoOfCartPerTrain;

    public void DeclareReferencedPrefabs(List<UnityGameObject> referencedPrefabs)
    {
        // Conversion only converts the GameObjects in the scene.
        // This function allows us to inject extra GameObjects,
        // in this case prefabs that live in the assets folder.
        referencedPrefabs.Add(LeaderPrefab);
        referencedPrefabs.Add(FollowerPrefab);
    }
    

    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        /*
        var allRenderers = transform.GetComponentsInChildren<UnityMeshRenderer>();
        var needBaseColor = new NativeArray<Entity>(allRenderers.Length, Allocator.Temp);
        
        for(int i = 0; i < allRenderers.Length; ++i)
        {
            var meshRenderer = allRenderers[i];
            needBaseColor[i] = conversionSystem.GetPrimaryEntity(meshRenderer.gameObject);
        }
        
        dstManager.AddComponent<URPMaterialPropertyBaseColor>(needBaseColor);
        */
        dstManager.AddComponentData(entity, new TrainSpawner
        {
            LeaderPrefab = conversionSystem.GetPrimaryEntity(LeaderPrefab),NoOfTrains=NoOfTrains,
            FollowerPrefab = conversionSystem.GetPrimaryEntity(FollowerPrefab),NoOfCartPerTrain=NoOfCartPerTrain
            
        });

    }
}
