using Unity.Entities;
using UnityEngine;

[RequiresEntityConversion]
public class PheromoneRendererAuthoringComponent : MonoBehaviour, IConvertGameObjectToEntity
{
    public MeshRenderer Renderer;
    public Material Material;
    public GameManager GameManager;
    
    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        dstManager.AddSharedComponentData(entity, new PheromoneRenderData
        {
            Material = Material,
            Renderer = Renderer
        });
        int size = GameManager.mapSize;
        dstManager.AddBuffer<PheromoneBuffer>(entity).ResizeUninitialized(size * size);
    }
}