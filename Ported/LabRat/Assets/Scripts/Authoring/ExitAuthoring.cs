using Unity.Collections;
using Unity.Entities;
using Unity.Rendering;
using UnityEngine;

public class ExitAuthoring : MonoBehaviour, IConvertGameObjectToEntity
{
    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        
        var allRenderers = transform.GetComponentsInChildren<MeshRenderer>();
        var needBaseColor = new NativeArray<Entity>(allRenderers.Length, Allocator.Temp);

        for (int i = 0; i < allRenderers.Length; i++)
        {
            var meshRenderer = allRenderers[i];
            needBaseColor[i] = conversionSystem.GetPrimaryEntity(meshRenderer.gameObject);
        }

        dstManager.AddComponent<URPMaterialPropertyBaseColor>(needBaseColor);

        dstManager.AddComponent<PropagateColor>(entity);
        dstManager.AddComponent<Tile>(entity);
        dstManager.AddComponent<Exit>(entity);
        dstManager.AddComponent<PlayerOwned>(entity);
    }
}